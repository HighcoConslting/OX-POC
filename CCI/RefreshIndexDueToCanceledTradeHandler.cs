namespace CCI
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus;
    using OX.Messages.CCI;

    public class RefreshIndexDueToCanceledTradeHandler : IHandleMessages<RefreshIndexDueToCanceledTrade>
    {
        public Task Handle(RefreshIndexDueToCanceledTrade message, IMessageHandlerContext context)
        {
            Console.WriteLine("Updating index (trade canceled)");

            var timestamp = DateTimeOffset.UtcNow.ToString();

            var @event = new IndexUpdated
            {
                ProductId = message.ProductId,
                OxiValue = $"OXI value updated at {timestamp}",
                OxdiValue = $"OXDI value updated at {timestamp}",
                BBLPerDay = $"BBL value updated at {timestamp}",
                M3 = $"M3 value updated at {timestamp}",
                Reason = "Trade canceled"
            };

            Console.WriteLine("Publishing index update");
            return context.Publish(@event);
        }
    }
}