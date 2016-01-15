using RestBus.Common.Amqp;
using RestBus.RabbitMQ;
using System.Linq;

namespace RestBusTestClient
{
    class SendOnlyMessageMapper : BasicMessageMapper
    {
        public SendOnlyMessageMapper(string amqpHostUri, string serviceName) : base(amqpHostUri, serviceName)
        {
        }

        public override ExchangeConfiguration GetExchangeConfig()
        {
            var connectionInfos = base.amqpHostUris.Select(u => new AmqpConnectionInfo { Uri = u, FriendlyName = base.StripUserInfoAndQuery(u) }).ToArray();
            return new ExchangeConfiguration(connectionInfos, base.serviceName)
            {
                MessageExpires = (m) => { return false; }, //Messages never expire
                MessageExpectsReply = (m) => { return false; } //Messages are not replied to
            };
        }

    }

}
