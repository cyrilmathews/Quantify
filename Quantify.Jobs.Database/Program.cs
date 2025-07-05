using DbUp;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Quantify.Jobs.Database
{
    public class Program
    {
        public static int Main(string[] args)
        {
            // 1. Set up configuration to read from appsettings.json
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // 2. Get the connection string from configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Connection string is not configured.");
                Console.ResetColor();
                return -1;
            }

            // 3. Configure DbUp
            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString) // Specify the database
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly()) // Find scripts in this assembly
                    .LogToConsole() // Log output to the console
                    .Build();

            Console.WriteLine("Database migration check started.");

            // 4. Check if any scripts need to be run
            var scriptsToExecute = upgrader.GetScriptsToExecute();
            if (!scriptsToExecute.Any())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Success! The database is already up to date.");
                Console.ResetColor();
                return 0;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("New scripts found. Starting upgrade...");
            Console.ResetColor();

            // 5. Perform the upgrade
            var result = upgrader.PerformUpgrade();

            // 6. Display the result
            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Database upgrade failed!");
                Console.WriteLine(result.Error);
                Console.ResetColor();
                return -1; // Return a non-zero exit code for failure
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success! The database has been upgraded.");
            Console.ResetColor();
            return 0; // Return 0 for success
        }
    }
}
