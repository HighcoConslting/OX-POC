namespace CCI
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus;
    using OX.Messages.CCI;

    public class RefreshIndexDueToNewTradeAddedHandler : IHandleMessages<RefreshIndexDueToNewTradeAdded>
    {
        public Task Handle(RefreshIndexDueToNewTradeAdded message, IMessageHandlerContext context)
        {
            Console.WriteLine("Updating index (new trade added)");

            var timestamp = DateTimeOffset.UtcNow.ToString();

            var @event = new IndexUpdated
            {
                ProductId = message.ProductId,
                OxiValue = $"OXI value updated at {timestamp}",
                OxdiValue = $"OXDI value updated at {timestamp}",
                BBLPerDay = $"BBL value updated at {timestamp}",
                M3 = $"M3 value updated at {timestamp}",
                Reason = "New trade added"
            };

            Console.WriteLine("Publishing index update");
            return context.Publish(@event);
        }
    }
}