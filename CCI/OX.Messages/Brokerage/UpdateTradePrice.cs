namespace OX.Messages.Brokerage
{
    using System;
    using NServiceBus;

    public class UpdateTradePrice : ICommand
    {
        public string ParticipantCode { get; set; }
        public int TradeId { get; set; }
        public DateTimeOffset UpdateDateTimeUtc { get; set; }
    }
}