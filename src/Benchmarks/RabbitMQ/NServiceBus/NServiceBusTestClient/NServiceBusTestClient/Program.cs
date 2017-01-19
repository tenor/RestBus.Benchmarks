﻿using NServiceBus;
using NServiceBus.Logging;
using NServiceBusTestCommon;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NServiceBusTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            LogManager.Use<DefaultFactory>().Level(LogLevel.Fatal); //Comment out to see log messages
            EndpointConfiguration busConfiguration = new EndpointConfiguration("nservicebus_test_client");
            busConfiguration.UseTransport<RabbitMQTransport>()
                .ConnectionString(ConfigurationManager.AppSettings["ServerConnectionString"])
                .PrefetchCount(50)
                .UsePublisherConfirms(false);
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.EnableInstallers();
            busConfiguration.DisableDurableMessages();
            busConfiguration.SendFailedMessagesTo("poop2");
//            busConfiguration.DiscardFailedMessagesInsteadOfSendingToErrorQueue();

            bool expectReply = Boolean.Parse(ConfigurationManager.AppSettings["ExpectReply"]);
            var taskCount = Int32.Parse(ConfigurationManager.AppSettings["NoOfThreads"]);
            var iterationsPerTask = Int32.Parse(ConfigurationManager.AppSettings["MessagesPerThread"]);

            var startableBus = Endpoint.Create(busConfiguration).Result;

            var bus = startableBus.Start().Result;


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
                            
//                            bus.Send("nservicebus_test_service", msg).Register((m) => { }).Wait();
                        }
                        else
                        {
                            bus.Send("nservicebus_test_service", msg);
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

            bus.Stop().Wait();


        }
    }
}
