namespace Brokerage
{
    using System;
    using OX.Messages.CCI;
    using System.Threading.Tasks;
    using NServiceBus;

    public class IndexUpdatedHandler : IHandleMessages<IndexUpdated>
    {
        public Task Handle(IndexUpdated message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Received index update for product: {message.ProductId}. Reason: {message.Reason}");
            Console.WriteLine($"OXI: {message.OxiValue}\tOXDI: {message.OxdiValue}\tBBL: {message.BBLPerDay}\tM3: {message.M3}");

            return Task.CompletedTask;
        }
    }
}