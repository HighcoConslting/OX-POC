using OX.Messages.Brokerage;

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

            transport.Routing().RouteToEndpoint(typeof(AddNewTrade), "CCI");
            transport.Routing().RouteToEndpoint(typeof(UpdateTradePrice), "CCI");
            transport.Routing().RouteToEndpoint(typeof(CancelTrade), "CCI");

            var endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine($"Brokerage '{brokerageId}' endpoint successfully started.");
            Console.WriteLine("Press N for new trade, C to cancel a trade, U to update a trade. Any other key to exit.");
            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.N:
                        var newTrade = new AddNewTrade
                        {
                            ParticipantCode = brokerageId,
                            TradeId = 1,
                            ProductId = "C5-FTSK",
                            Quantity = 10000,
                            Currency = "CAD",
                            IsMetric = true,
                            Price = 25000,
                            TradeDateTimeUtc = DateTimeOffset.UtcNow
                        };
                        await endpoint.Send(newTrade).ConfigureAwait(false);
                        break;

                    default:
                        await endpoint.Stop().ConfigureAwait(false);
                        return;
                }
            }
        }
    }
}
