using System.Configuration;
using Shuttle.Esb;
using ShuttleTestCommon;

namespace MassTransitTestServer
{
    public class MessageHandler : IMessageHandler<Message>
    {
        private static readonly bool Reply = bool.Parse(ConfigurationManager.AppSettings["Reply"] ?? "false");

        public void ProcessMessage(IHandlerContext<Message> context)
        {
            if (!Reply)
            {
                return;
            }

            context.Send(new Message {Body = BodyGenerator.GetNext()}, c => c.Reply());
        }
    }
}