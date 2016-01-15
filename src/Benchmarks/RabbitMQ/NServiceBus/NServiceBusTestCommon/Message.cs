using NServiceBus;

namespace NServiceBusTestCommon
{
    public class Message : IMessage
    {
        public byte[] Body;
    }
}
