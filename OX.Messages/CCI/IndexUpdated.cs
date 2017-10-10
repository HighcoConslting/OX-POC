namespace OX.Messages.CCI
{
    using NServiceBus;

    public class IndexUpdated : IEvent
    {
        public string ProductId { get; set; }
        public string OxiValue { get; set; }
        public string OxdiValue { get; set; }
        public string BBLPerDay { get; set; }
        public string M3 { get; set; }
        public string Reason { get; set; }
    }
}