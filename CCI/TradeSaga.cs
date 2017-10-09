using System.Threading.Tasks;
using NServiceBus;
using OX.Messages.Brokerage;
using OX.Messages.CCI;

namespace CCI
{
    public class TradeSaga : Saga<TradeState>, 
        IAmStartedByMessages<AddNewTrade>, 
        IHandleMessages<UpdateTradePrice>, 
        IHandleMessages<CancelTrade>, 
        IHandleMessages<CompleteTradeSaga>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<TradeState> mapper)
        {
            mapper.ConfigureMapping<AddNewTrade>(m => $"{m.ParticipantCode}_{m.TradeId}") 
                .ToSaga(state => state.UniqueTradeId);

            mapper.ConfigureMapping<UpdateTradePrice>(m => $"{m.ParticipantCode}_{m.TradeId}") 
                .ToSaga(state => state.UniqueTradeId);

            mapper.ConfigureMapping<CancelTrade>(m => $"{m.ParticipantCode}_{m.TradeId}")
                .ToSaga(state => state.UniqueTradeId);

            mapper.ConfigureMapping<CompleteTradeSaga>(m => $"{m.ParticipantCode}_{m.TradeId}")
                .ToSaga(state => state.UniqueTradeId);
        }

        public Task Handle(AddNewTrade message, IMessageHandlerContext context)
        {
            Data.TradeId = message.TradeId;
            Data.ParticipantCode = message.ParticipantCode;
            Data.ProductId = message.ProductId;
            Data.Quantity = message.Quantity;
            Data.Price = message.Price;
            Data.Currency = message.Currency;
            Data.IsMetric = message.IsMetric;
            Data.TradeDateTimeUtc = message.TradeDateTimeUtc;

            var refreshIndexDueToNewTradeAdded = new RefreshIndexDueToNewTradeAdded
            {
                ProductId = Data.ProductId,
                ParticipantCode = Data.ParticipantCode,
                TradeId = Data.TradeId,
                Price = Data.Price
            };

            return context.SendLocal(refreshIndexDueToNewTradeAdded);
        }

        public Task Handle(UpdateTradePrice message, IMessageHandlerContext context)
        {
            Data.Price = message.NewPrice;

            var refreshIndexDueToUpdatedTradePrice = new RefreshIndexDueToUpdatedTradePrice
            {
                ProductId = Data.ProductId,
                ParticipantCode = Data.ParticipantCode,
                TradeId = Data.TradeId,
                Price = Data.Price
            };

            return context.SendLocal(refreshIndexDueToUpdatedTradePrice);
        }

        public async Task Handle(CancelTrade message, IMessageHandlerContext context)
        {
            var refreshIndexDueToCanceledTrade = new RefreshIndexDueToCanceledTrade
            {
                TradeId = Data.TradeId,
                ParticipantCode = Data.ParticipantCode,
                ProductId = Data.ProductId,
                Quantity = Data.Quantity,
                Price = Data.Price,
                Currency = Data.Currency
            };

            await context.SendLocal(refreshIndexDueToCanceledTrade).ConfigureAwait(false);

            var completeTradeSaga = new CompleteTradeSaga
            {
                ParticipantCode = Data.ParticipantCode,
                TradeId = Data.TradeId
            };
            await context.SendLocal(completeTradeSaga).ConfigureAwait(false);
        }

        public Task Handle(CompleteTradeSaga message, IMessageHandlerContext context)
        {
            MarkAsComplete();
            return Task.CompletedTask;
        }
    }
}