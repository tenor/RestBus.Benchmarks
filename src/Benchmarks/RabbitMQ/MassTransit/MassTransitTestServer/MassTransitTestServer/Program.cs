using MassTransit;
using MassTransitTestCommon;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace MassTransitTestServer
{
    class Program
    {
        static void Main(string[] args)
        {            
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
           {
               var host = cfg.Host(new Uri(ConfigurationManager.AppSettings["ServerUri"]), h =>
               {
                   h.Username(ConfigurationManager.AppSettings["Username"]);
                   h.Password(ConfigurationManager.AppSettings["Password"]);

               });

               cfg.ReceiveEndpoint(host, "masstransit_message_service", e =>
               {
                   //e.AutoDelete = true; //Client doesn't play well with this setting!
                   //e.Durable = false; //Client doesn't play well with this setting!
                   e.PrefetchCount = 50;
                   e.Consumer<MessageConsumer>();
               });
           });

            busControl.Start();

            Console.WriteLine("Server started ... Ctrl+C to exit.");
            Console.ReadKey();
        }
    }


    public class MessageConsumer : IConsumer<Message>
    {
        static bool reply = bool.Parse(ConfigurationManager.AppSettings["Reply"]);

        public async Task Consume(ConsumeContext<Message> context)
        {
            if (reply)
            {
                await context.RespondAsync(new Message { Body = BodyGenerator.GetNext() });
            }
        }
    }
}
