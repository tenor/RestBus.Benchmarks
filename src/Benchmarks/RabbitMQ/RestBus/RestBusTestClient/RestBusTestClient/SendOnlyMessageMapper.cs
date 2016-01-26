using RestBus.Common.Amqp;
using RestBus.RabbitMQ;

namespace RestBusTestClient
{
    class SendOnlyMessageMapper : BasicMessageMapper
    {
        public SendOnlyMessageMapper(string amqpHostUri, string serviceName) : base(amqpHostUri, serviceName)
        {
        }

        public override MessagingConfiguration MessagingConfig
        {
            get
            {
                return new MessagingConfiguration
                {
                    MessageExpires = (m) => { return false; }, //Messages never expire
                    MessageExpectsReply = (m) => { return false; } //Messages are not replied to
                };
            }
        }

    }

}
