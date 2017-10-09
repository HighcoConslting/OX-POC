namespace CCI
{
    using System;
    using NServiceBus;

    public class TradeState : ContainSagaData
    {
        public string UniqueTradeId { get; set; }
        public int TradeId { get; set; }
        public string ParticipantCode { get; set; }
        public string ProductId { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public bool IsMetric { get; set; }
        public DateTimeOffset TradeDateTimeUtc { get; set; }
    }
}