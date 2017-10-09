namespace Brokerage
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;
    using NServiceBus;
    using NServiceBus.Logging;
    using NServiceBus.Transport.AzureServiceBus;

    static class BrokerageProgram
    {
        static async Task Main(string[] args)
        {
            var brokerageId = ConfigurationManager.AppSettings["Brokerage.ID"];
            if (string.IsNullOrWhiteSpace(brokerageId))
            {
                throw new Exception("Did not find Brokerage ID 'Brokerage.ID' in App.config file.");
            }
            var brokerage = "Broker-" + brokerageId;
            
            Console.Title = brokerage;
            Console.WriteLine($"Starting brokerage '{brokerageId}' endpoint... ");
            var defaultFactory = LogManager.Use<DefaultFactory>();
            defaultFactory.Level(LogLevel.Warn);

            var endpointConfiguration = new EndpointConfiguration(brokerage);

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

            Console.WriteLine($"Brokerage '{brokerageId}' endpoint successfully started.");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            await endpoint.Stop().ConfigureAwait(false);
        }
    }
}
