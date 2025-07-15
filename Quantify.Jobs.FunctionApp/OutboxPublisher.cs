using Azure.Messaging.ServiceBus;
using Dapper;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Quantify.Jobs.FunctionApp;

public class OutboxPublisher
{
    private readonly ILogger<OutboxPublisher> _logger;
    private readonly string _dbConnectionString;
    private readonly string _serviceBusConnectionString;
    private readonly string _serviceBusTopicName;

    public OutboxPublisher(ILogger<OutboxPublisher> logger, IConfiguration configuration)
    {
        _logger = logger;
        _dbConnectionString = configuration["SqlDbConnection"] ?? string.Empty;
        _serviceBusConnectionString = configuration["ServiceBusConnection"] ?? string.Empty;
        _serviceBusTopicName = configuration["ServiceBusTopicName"] ?? string.Empty;

        ValidateConfiguration();
    }

    [Function("OutboxPublisher")]
    public async Task Run([TimerTrigger("*/5 * * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation($"OutboxPublisher function started at: {DateTime.Now}");

        await using var serviceBusClient = new ServiceBusClient(_serviceBusConnectionString);
        await using var serviceBusSender = serviceBusClient.CreateSender(_serviceBusTopicName);

        var eventsToPublish = await FetchUnprocessedEventsAsync();

        _logger.LogInformation($"Found {eventsToPublish.Count} unprocessed events in the outbox.");

        foreach (var outboxEvent in eventsToPublish)
        {
            var published = await TryPublishEventAsync(serviceBusSender, outboxEvent);
            if (published)
            {
                await MarkEventAsProcessedAsync(outboxEvent.Id);
                _logger.LogInformation($"Event {outboxEvent.Id} marked as processed in outbox.");
            }
        }

        _logger.LogInformation($"OutboxPublisher function finished at: {DateTime.Now}");
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrEmpty(_dbConnectionString))
        {
            _logger.LogError("SqlDbConnection is not configured.");
            throw new InvalidOperationException("SqlDbConnection is not configured.");
        }
        if (string.IsNullOrEmpty(_serviceBusConnectionString))
        {
            _logger.LogError("ServiceBusConnection is not configured.");
            throw new InvalidOperationException("ServiceBusConnection is not configured.");
        }
        if (string.IsNullOrEmpty(_serviceBusTopicName))
        {
            _logger.LogError("ServiceBusTopicName is not configured.");
            throw new InvalidOperationException("ServiceBusTopicName is not configured.");
        }
    }

    private async Task<List<OutboxEvent>> FetchUnprocessedEventsAsync()
    {
        using var dbConnection = new SqlConnection(_dbConnectionString);
        await dbConnection.OpenAsync();
        var sql = "SELECT Id, EventType, EventData FROM Outbox WHERE IsProcessed = 0 ORDER BY Timestamp ASC";
        var events = await dbConnection.QueryAsync<OutboxEvent>(sql);
        return events.AsList();
    }

    private async Task<bool> TryPublishEventAsync(ServiceBusSender sender, OutboxEvent outboxEvent)
    {
        try
        {
            var message = new ServiceBusMessage(outboxEvent.EventData)
            {
                ContentType = "application/json",
                Subject = outboxEvent.EventType,
                MessageId = outboxEvent.Id.ToString()
            };

            _logger.LogInformation($"Publishing event {outboxEvent.Id} ({outboxEvent.EventType}) to Service Bus topic {_serviceBusTopicName}...");
            await sender.SendMessageAsync(message);
            _logger.LogInformation($"Event {outboxEvent.Id} published successfully.");
            return true;
        }
        catch (ServiceBusException sbEx)
        {
            _logger.LogError(sbEx, $"Service Bus error publishing event {outboxEvent.Id}: {sbEx.Message}. This event will be retried on the next function run.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"General error processing outbox event {outboxEvent.Id}: {ex.Message}. This event will be retried on the next function run.");
            return false;
        }
    }

    private async Task MarkEventAsProcessedAsync(Guid eventId)
    {
        using var dbConnection = new SqlConnection(_dbConnectionString);
        await dbConnection.OpenAsync();
        var updateSql = "UPDATE Outbox SET IsProcessed = 1, ProcessedDate = GETUTCDATE() WHERE Id = @Id";
        await dbConnection.ExecuteAsync(updateSql, new { Id = eventId });
    }

    private class OutboxEvent
    {
        public Guid Id { get; set; }
        public string EventType { get; set; }
        public string EventData { get; set; }
    }
}
