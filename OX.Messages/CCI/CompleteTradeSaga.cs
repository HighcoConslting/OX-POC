namespace OX.Messages.CCI
{
    using NServiceBus;

    public class CompleteTradeSaga : ICommand
    {
        public int TradeId { get; set; }
        public string ParticipantCode { get; set; }
    }
}