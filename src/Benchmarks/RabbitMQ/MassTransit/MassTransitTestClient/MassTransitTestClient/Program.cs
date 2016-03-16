using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using MassTransit;
using MassTransitTestCommon;

namespace MassTransitTestClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var taskCount = int.Parse(ConfigurationManager.AppSettings["NoOfThreads"]);

            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.PrefetchCount = 50;
                cfg.Durable = false;
                cfg.AutoDelete = true;
                cfg.PublisherConfirmation = false;

                var host = cfg.Host(new Uri(ConfigurationManager.AppSettings["ServerUri"]), h =>
                {
                    h.Username(ConfigurationManager.AppSettings["Username"]);
                    h.Password(ConfigurationManager.AppSettings["Password"]);
                });
            });

            busControl.Start();
            var timeout = TimeSpan.FromSeconds(60);
            var expectReply = bool.Parse(ConfigurationManager.AppSettings["ExpectReply"]);
            var serviceUri =
                new Uri(ConfigurationManager.AppSettings["ServerUri"] +
                        (!expectReply
                            ? "masstransit_message_service"
                            : "masstransit_message_service_rpc?durable=false&autodelete=true"));
            var iterationsPerTask = int.Parse(ConfigurationManager.AppSettings["MessagesPerThread"]);

            IRequestClient<Message, Message> client = null;
            if (expectReply)
            {
                client = busControl.CreateRequestClient<Message, Message>(serviceUri, timeout);
            }

            var tasks = new Task[taskCount];
            for (var t = 0; t < taskCount; t++)
            {
                tasks[t] = new Task(() =>
                {
                    Message msg;
                    for (var i = 0; i < iterationsPerTask; i++)
                    {
                        msg = new Message {Body = BodyGenerator.GetNext()};
                        if (expectReply)
                        {
                            var res = client.Request(msg).Result;
                        }
                        else
                        {
                            busControl.Publish(msg).Wait();
                        }
                    }
                }, TaskCreationOptions.LongRunning);
            }

            Console.WriteLine("Sending " + iterationsPerTask*taskCount + " messages across " + taskCount + " threads");
            var watch = new Stopwatch();
            watch.Start();
            for (var t = 0; t < taskCount; t++)
            {
                tasks[t].Start();
            }

            Task.WaitAll(tasks);

            watch.Stop();
            Console.WriteLine("Total time: " + watch.Elapsed);

            Console.ReadKey();
            busControl.Stop();
        }
    }
}