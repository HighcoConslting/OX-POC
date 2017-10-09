namespace OX.Messages.CCI
{
    using NServiceBus;

    public class IndexUpdated : IEvent
    {
        public string ProductId { get; set; }
        public double OxiValue { get; set; }
        public double OxdiValue { get; set; }
        public string BBLPerDay { get; set; }
        public string M3 { get; set; }
    }
}