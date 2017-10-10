namespace OX.Messages.CCI
{
    using NServiceBus;

    public class RefreshIndexDueToUpdatedTradePrice : ICommand
    {
        public string ParticipantCode { get; set; }
        public int TradeId { get; set; }
        public string ProductId { get; set; }
        public double Price { get; set; }
    }
}