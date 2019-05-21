using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using Ninject;
using Shuttle.Core.Ninject;
using Shuttle.Esb;
using ShuttleTestCommon;

namespace ShuttleTestClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var iterationsPerTask = int.Parse(ConfigurationManager.AppSettings["MessagesPerThread"]);
            var taskCount = int.Parse(ConfigurationManager.AppSettings["NoOfThreads"]);

            var container = new NinjectComponentContainer(new StandardKernel());

            ServiceBus.Register(container);

            using (var bus = ServiceBus.Create(container).Start())
            {
                var tasks = new Task[taskCount];

                for (var t = 0; t < taskCount; t++)
                {
                    tasks[t] = new Task(() =>
                    {
                        for (var i = 0; i < iterationsPerTask; i++)
                        {
                            bus.Send(new Message {Body = BodyGenerator.GetNext()});
                        }
                    }, TaskCreationOptions.LongRunning);
                }

                Console.WriteLine("Sending " + iterationsPerTask * taskCount + " messages across " + taskCount + " threads");

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
            }
        }
    }
}