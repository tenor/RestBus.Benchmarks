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

               var reply = bool.Parse(ConfigurationManager.AppSettings["Reply"] ?? "false");

               var queueName = !reply ? "masstransit_message_service" : "masstransit_message_service_rpc";
               cfg.ReceiveEndpoint(host, queueName, e =>
               {
                   e.AutoDelete = true; 
                   e.Durable = false;
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
        private static bool reply = bool.Parse(ConfigurationManager.AppSettings["Reply"] ?? "false");

        public async Task Consume(ConsumeContext<Message> context)
        {
            if (reply)
            {
                await context.RespondAsync(new Message { Body = BodyGenerator.GetNext() });
            }
        }
    }
}
