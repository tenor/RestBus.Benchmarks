using EasyNetQ;
using EasyNetQTestCommon;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EasyNetQTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            int iterationsPerTask = Int32.Parse(ConfigurationManager.AppSettings["MessagesPerThread"]);
            int taskCount = Int32.Parse(ConfigurationManager.AppSettings["NoOfThreads"]);
            bool expectReply = Boolean.Parse(ConfigurationManager.AppSettings["ExpectReply"]);

            var bus = RabbitHutch.CreateBus(ConfigurationManager.AppSettings["ServerConnectionString"]);


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
                            bus.Request<Message, Message>(msg);
                        }
                        else
                        {
                            bus.Send<Message>( "easynetq_queue" , msg);
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
            bus.Dispose();


        }
    }


}
