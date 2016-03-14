using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using RawRabbit.Common;
using RawRabbit.Common.Messages;

namespace RawRabbit.Client
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var iterationsPerTask = int.Parse(ConfigurationManager.AppSettings["MessagesPerThread"]);
			var taskCount = int.Parse(ConfigurationManager.AppSettings["NoOfThreads"]);
			var expectReply = bool.Parse(ConfigurationManager.AppSettings["ExpectReply"]);
			var msgSize = int.Parse(ConfigurationManager.AppSettings["MessageSize"]);

			var bodyGenerator = new BodyGenerator(msgSize);
			var client = BenchmarkClientFactory.CreateBusClient();

			var tasks = new Task[taskCount];
			for (var t = 0; t < taskCount; t++)
			{
				tasks[t] = new Task(() =>
				{
					var msg = new Message {Body = bodyGenerator.GetNext()};
					var inner = new Task[iterationsPerTask];
					for (var i = 0; i < iterationsPerTask; i++)
					{
						if (expectReply)
						{
							inner[i] = client.RequestAsync<Message, Message>(msg);
						}
						else
						{
							inner[i] = client.PublishAsync(msg);
						}
					}
					Task.WaitAll(inner);
				}, TaskCreationOptions.LongRunning);
			}

			Console.WriteLine("Preparing to send " + iterationsPerTask * taskCount + " messages across " + taskCount + " threads");
			Console.WriteLine("Press [enter] to start");
			Console.ReadKey();
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
			client.Dispose();
		}
	}
}
