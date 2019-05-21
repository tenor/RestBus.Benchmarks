using Shuttle.Esb;
using ShuttleTestCommon;

namespace MassTransitTestClient
{
    public class MessageHandler : IMessageHandler<Message>
    {
        public void ProcessMessage(IHandlerContext<Message> context)
        {
        }
    }
}