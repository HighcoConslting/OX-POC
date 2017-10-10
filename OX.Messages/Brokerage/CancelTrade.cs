using System;

namespace OX.Messages.Brokerage
{
    using NServiceBus;

    public class CancelTrade : ICommand
    {
        public string ParticipantCode { get; set; }
        public int TradeId { get; set; }
        public DateTimeOffset CancellationDateTimeUtc { get; set; }
    }
}