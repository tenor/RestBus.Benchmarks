namespace NServiceBusTestServer
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;
    using NServiceBus;
    using NServiceBus.Logging;
    using NServiceBusTestCommon;


    class Program
    {
        static void Main(string[] args)
        {
            LogManager.Use<DefaultFactory>().Level(LogLevel.Fatal); //Comment out to see log messages.
            var busConfiguration = new EndpointConfiguration("nservicebus_test_service");
            busConfiguration.UseTransport<RabbitMQTransport>()
                .ConnectionString(ConfigurationManager.AppSettings["ServerConnectionString"])
                .PrefetchCount(50)
                .UsePublisherConfirms(false);
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.DisableDurableMessages();
            busConfiguration.SendFailedMessagesTo("poop");

            var startableBus = Endpoint.Create(busConfiguration).Result;

            var bus = startableBus.Start().Result;

            Console.WriteLine("Server started ... Press any key to exit");
            Console.ReadKey();

            bus.Stop().Wait();
        }
    }


    public class RequestDataMessageHandler :
        IHandleMessages<Message>
    {
        static bool reply = bool.Parse(ConfigurationManager.AppSettings["Reply"]);

        public async Task Handle(Message message, IMessageHandlerContext context)
        {
            if (reply)
            {
                var response = new Message
                {
                    Body = BodyGenerator.GetNext()
                };

                context.Reply(response);
            }
        }

        public void Handle(Message message)
        {
        }
    }
}