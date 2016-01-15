using NServiceBus;
using NServiceBus.Logging;
using NServiceBusTestCommon;
using System;
using System.Configuration;

namespace NServiceBusTestServer
{
    class Program
    {
        static void Main(string[] args)
        {

            LogManager.Use<DefaultFactory>().Level(LogLevel.Fatal); //Comment out to see log messages.
            BusConfiguration busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("nservicebus_test_service");
            busConfiguration.UseTransport<RabbitMQTransport>()
                .ConnectionString(ConfigurationManager.AppSettings["ServerConnectionString"]);
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.DisableDurableMessages();
            busConfiguration.DiscardFailedMessagesInsteadOfSendingToErrorQueue();

            using (IBus bus = Bus.Create(busConfiguration).Start())
            {
                Console.WriteLine("Server started ... Press any key to exit");
                Console.ReadKey();
            }
        }
    }

    public class RequestDataMessageHandler : IHandleMessages<Message>
    {
        static bool reply = bool.Parse(ConfigurationManager.AppSettings["Reply"]);

        IBus bus;

        public RequestDataMessageHandler(IBus bus)
        {
            this.bus = bus;
        }

        public void Handle(Message message)
        {
            if (reply)
            {
                Message response = new Message
                {
                    Body = BodyGenerator.GetNext()
                };

                bus.Reply(response);
            }
        }
    }



}
