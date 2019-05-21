using Shuttle.Esb;
using ShuttleTestCommon;

namespace ShuttleTestClient
{
    public class MessageHandler : IMessageHandler<Message>
    {
        public void ProcessMessage(IHandlerContext<Message> context)
        {
        }
    }
}