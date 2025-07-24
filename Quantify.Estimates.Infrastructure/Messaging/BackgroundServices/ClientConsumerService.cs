using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quantify.Estimates.Core.Events.Client;
using System.Text.Json;

namespace Quantify.Estimates.Infrastructure.Messaging.BackgroundServices
{
    public class ClientConsumerService : BackgroundService
    {
        private readonly ILogger<ClientConsumerService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private ServiceBusClient _serviceBusClient;
        private ServiceBusProcessor _serviceBusProcessor;
        private readonly string _serviceBusConnectionString;
        private readonly string _serviceBusTopicName;
        private readonly string _serviceBusSubscriptionName;
        private readonly Dictionary<string, Func<string, string, CancellationToken, Task>> _eventHandlers;

        public ClientConsumerService(
            ILogger<ClientConsumerService> logger, 
            IConfiguration configuration, 
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _serviceBusConnectionString = _configuration["ServiceBusConnection"] ?? string.Empty;
            _serviceBusTopicName = _configuration.GetSection("ServiceBusTopicNames")["ClientEvents"] ?? string.Empty;
            _serviceBusSubscriptionName = _configuration.GetSection("ServiceBusSubscriptionNames")["ClientSubscription"] ?? string.Empty;
            _eventHandlers = GetEventHandlers();

            ValidateConfiguration();
        }

        private Dictionary<string, Func<string, string, CancellationToken, Task>> GetEventHandlers()
        {
            return new Dictionary<string, Func<string, string, CancellationToken, Task>>
            {
                [nameof(ClientCreatedEvent)] = async (body, messageId, cancellationToken) =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    var eventDispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();
                    await eventDispatcher.Dispatch(DeserializeEvent<ClientCreatedEvent>(body, messageId), cancellationToken);
                }
            };
        }

        private void ValidateConfiguration()
        {
            if (string.IsNullOrEmpty(_serviceBusConnectionString))
            {
                _logger.LogError("Configuration setting 'ServiceBusConnection' is missing for ClientConsumerService. Please check appsettings.json.");
                throw new InvalidOperationException("Missing configuration: ServiceBusConnection");
            }
            if (string.IsNullOrEmpty(_serviceBusTopicName))
            {
                _logger.LogError("Configuration setting 'ServiceBusTopicName' is missing for ClientConsumerService. Please check appsettings.json.");
                throw new InvalidOperationException("Missing configuration: ServiceBusTopicName");
            }
            if (string.IsNullOrEmpty(_serviceBusSubscriptionName))
            {
                _logger.LogError("Configuration setting 'ServiceBusSubscriptionName' is missing for ClientConsumerService. Please check appsettings.json.");
                throw new InvalidOperationException("Missing configuration: ServiceBusSubscriptionName");
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ClientConsumerService is starting.");

            try
            {
                _serviceBusClient = new ServiceBusClient(_serviceBusConnectionString);
                _serviceBusProcessor = _serviceBusClient.CreateProcessor(_serviceBusTopicName, _serviceBusSubscriptionName, new ServiceBusProcessorOptions { MaxConcurrentCalls = 1, AutoCompleteMessages = false });
                _serviceBusProcessor.ProcessMessageAsync += MessageHandler;
                _serviceBusProcessor.ProcessErrorAsync += ErrorHandler;
                await _serviceBusProcessor.StartProcessingAsync(cancellationToken);
                _logger.LogInformation($"Processor for '{_serviceBusSubscriptionName}' started.");
                _logger.LogInformation("Consumer is now listening.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start ClientConsumerService.");
                throw;
            }

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // This method just keeps the service alive. Processors handle continuous listening.
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            _logger.LogInformation("ClientConsumerService is stopping.");
        }

        // This method is called when the host stops.
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service B Consumer Background Service is performing graceful shutdown.");

            if (_serviceBusProcessor != null)
            {
                await _serviceBusProcessor.StopProcessingAsync();
                await _serviceBusProcessor.DisposeAsync();
            }
            if (_serviceBusClient != null)
            {
                await _serviceBusClient.DisposeAsync();
            }

            await base.StopAsync(cancellationToken);
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            string messageId = args.Message.MessageId;
            string subject = args.Message.Subject;
            var cancellationToken = args.CancellationToken;

            _logger.LogInformation($"ClientConsumerService: Received message: Id = {messageId}, Subject = {subject}, Body = {body}");

            try
            {
                if (_eventHandlers.TryGetValue(subject, out var handler))
                {
                    await handler(body, messageId, cancellationToken);
                }
                else
                {
                    _logger.LogWarning($"ClientConsumerService: Unknown event subject '{subject}' for message ID {messageId}. Moving to DLQ.");
                    await args.DeadLetterMessageAsync(args.Message, "UnknownSubject", $"Unknown event subject: {subject}");
                    return;
                }

                await args.CompleteMessageAsync(args.Message);
                _logger.LogInformation($"ClientConsumerService: Message {messageId} completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ClientConsumerService: Error processing message {messageId}. Moving to Dead-Letter Queue.");
                await args.DeadLetterMessageAsync(args.Message, "ProcessingFailure", ex.Message);
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, $"ClientConsumerService: Error in Service Bus processor. Error Source: {args.ErrorSource}, Entity Path: {args.EntityPath}");
            return Task.CompletedTask;
        }

        private TEvent DeserializeEvent<TEvent>(string eventBody, string messageId)
        {
            try
            {
                return JsonSerializer.Deserialize<TEvent>(eventBody);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, $"ClientConsumerService: Failed to deserialize {typeof(TEvent).Name} for message ID {messageId}.");
                throw; // Re-throw to dead-letter
            }
        }
    }
}
