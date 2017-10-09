namespace CCI
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;
    using NServiceBus;
    using NServiceBus.Logging;
    using NServiceBus.Persistence;
    using NServiceBus.Transport.AzureServiceBus;

    static class CCIProgram
    {
        static async Task Main(string[] args)
        {
            Console.Title = "CCI";
            Console.WriteLine("Starting CCI endpoint... ");
            var defaultFactory = LogManager.Use<DefaultFactory>();
            defaultFactory.Level(LogLevel.Warn);

            var endpointConfiguration = new EndpointConfiguration("CCI");

            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            var asbConnectionString = ConfigurationManager.AppSettings["AzureServiceBus.ConnectionString"];
            if (string.IsNullOrWhiteSpace(asbConnectionString))
            {
                throw new Exception("Did not find Azure Service Bus connection string 'AzureServiceBus.ConnectionString' in App.config file.");
            }
            transport.ConnectionString(asbConnectionString);

            transport.BrokeredMessageBodyType(SupportedBrokeredMessageBodyTypes.Stream);
            transport.UseForwardingTopology();

            var persistence = endpointConfiguration.UsePersistence<AzureStoragePersistence, StorageType.Sagas>();
            var storageConnectionString = ConfigurationManager.AppSettings["AzureStorage.ConnectionString"];
            if (string.IsNullOrWhiteSpace(storageConnectionString))
            {
                throw new Exception("Did not find Azure Storage connection string 'AzureStorage.ConnectionString' in App.config file.");
            }
            persistence.ConnectionString(storageConnectionString);
            
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.EnableInstallers();

            endpointConfiguration.DefineCriticalErrorAction(criticalErrorContext =>
            {
                Console.WriteLine($"Critical error has been encountered: {criticalErrorContext.Error}\nException: {criticalErrorContext.Exception}");
                Console.WriteLine("Taking the default action (shutdown for now).");
                Environment.Exit(-1);
                return Task.CompletedTask;
            });

            var endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine("CCI endpoint successfully started.");
            Console.WriteLine("Press any key to exit.");

            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();

                if (key.Key != ConsoleKey.Enter)
                {
                    break;
                }
            }

            await endpoint.Stop().ConfigureAwait(false);
        }
    }
}
