namespace OX.Messages.CCI
{
    using NServiceBus;

    public class ProductTotalsUpdated : IEvent
    {
        public string ProductId { get; set; }
        public int Total { get; set; }
    }
}