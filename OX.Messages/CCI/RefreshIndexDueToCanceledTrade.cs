namespace OX.Messages.CCI
{
    using NServiceBus;

    public class RefreshIndexDueToCanceledTrade : ICommand
    {
        public string ParticipantCode { get; set; }
        public int TradeId { get; set; }
        public string ProductId { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public double Quantity { get; set; }
    }
}