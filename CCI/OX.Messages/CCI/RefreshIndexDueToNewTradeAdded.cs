namespace OX.Messages.CCI
{
    using NServiceBus;

    public class RefreshIndexDueToNewTradeAdded : ICommand
    {
        public string ParticipantCode { get; set; }
        public int TradeId { get; set; }
        public string ProductId { get; set; }
        public double Price { get; set; }
    }
}