using MassTransit;
using MassTransitTestCommon;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MassTransitTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                //Commented out settings below had no effect on client
                //cfg.AutoDelete = true;
                //cfg.Durable = false;
                //cfg.SetQueueArgument("durable", false);

                //Also there seems to be no way to turn off publisher confirms.

                cfg.PrefetchCount = 50;

                var host = cfg.Host(new Uri(ConfigurationManager.AppSettings["ServerUri"]), h =>
                {
                    h.Username(ConfigurationManager.AppSettings["Username"]);
                    h.Password(ConfigurationManager.AppSettings["Password"]);
                });
            });

            busControl.Start();
            var timeout = TimeSpan.FromSeconds(60);
            var serviceUri = new Uri(ConfigurationManager.AppSettings["ServerUri"] + "masstransit_message_service");
            var expectReply = Boolean.Parse(ConfigurationManager.AppSettings["ExpectReply"]);
            var taskCount = Int32.Parse(ConfigurationManager.AppSettings["NoOfThreads"]);
            var iterationsPerTask = Int32.Parse(ConfigurationManager.AppSettings["MessagesPerThread"]);

            IRequestClient<Message, Message> client = null;
            if (expectReply)
            {
                client = busControl.CreateRequestClient<Message, Message>(serviceUri, timeout);
            }

            Task[] tasks = new Task[taskCount];
            for (int t = 0; t < taskCount; t++)
            {
                tasks[t] = new Task(() =>
                {
                    Message msg;
                    for (int i = 0; i < iterationsPerTask; i++)
                    {
                        msg = new Message { Body = BodyGenerator.GetNext() };
                        if (expectReply)
                        {
                            var res = client.Request(msg).Result;
                        }
                        else
                        {
                            busControl.Publish<Message>(msg).Wait();
                        }
                    }

                }, TaskCreationOptions.LongRunning);
            }

            Console.WriteLine("Sending " + iterationsPerTask * taskCount + " messages across " + taskCount + " threads");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int t = 0; t < taskCount; t++)
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
