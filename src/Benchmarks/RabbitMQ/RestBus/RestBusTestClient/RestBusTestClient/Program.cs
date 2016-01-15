using RestBus.Client;
using RestBus.RabbitMQ;
using RestBus.RabbitMQ.Client;
using RestBus.RabbitMQ.Subscription;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RestBusTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            int iterationsPerTask = Int32.Parse(ConfigurationManager.AppSettings["MessagesPerThread"]);
            int taskCount = Int32.Parse(ConfigurationManager.AppSettings["NoOfThreads"]);
            bool expectReply = Boolean.Parse(ConfigurationManager.AppSettings["ExpectReply"]);

            var amqpUrl = ConfigurationManager.AppSettings["ServerUri"]; //AMQP URI for RabbitMQ server
            var serviceName = "speedtest"; //Uniquely identifies this service

            var msgMapper = expectReply ? new BasicMessageMapper(amqpUrl, serviceName) : new SendOnlyMessageMapper(amqpUrl, serviceName);
            var subscriber = new RestBusSubscriber(msgMapper);

            var client = new RestBusClient(msgMapper);

            Task[] tasks = new Task[taskCount];
            for(int t = 0; t < taskCount; t++)
            {
                tasks[t] = new Task(() =>
                {
                    Message msg;
                    for (int i = 0; i < iterationsPerTask; i++)
                    {
                        msg = new Message { Body = BodyGenerator.GetNext() };
                        var res = client.PostAsJsonAsync("api/test/", msg, null).Result;
                    }

                }, TaskCreationOptions.LongRunning);
            }

            Console.WriteLine("Sending " + iterationsPerTask * taskCount + " messages across " + taskCount + " threads");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for(int t = 0; t < taskCount; t++)
            {
                tasks[t].Start();
            }

            Task.WaitAll(tasks);

            watch.Stop();
            Console.WriteLine("Total time: " + watch.Elapsed);
            Console.ReadKey();
            client.Dispose();
        }
    }

    public class Message
    {
        public byte[] Body;
    }


}
