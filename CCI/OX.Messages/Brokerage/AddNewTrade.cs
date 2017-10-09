using System;

namespace OX.Messages.Brokerage
{
    using NServiceBus;

    public class AddNewTrade : ICommand
    {
        public string ParticipantCode { get; set; }
        public int TradeId { get; set; }
        public DateTimeOffset TradeDateTimeUtc { get; set; }
        public string ProductId { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public double Quantity { get; set; }
        public bool IsMetric { get; set; }
    }
}