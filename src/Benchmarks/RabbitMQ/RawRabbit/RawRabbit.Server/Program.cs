using System;
using System.Configuration;
using System.Threading.Tasks;
using RawRabbit.Common;
using RawRabbit.Common.Messages;

namespace RawRabbit.Server
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var msgSize = int.Parse(ConfigurationManager.AppSettings["MessageSize"]);
			var rpc = bool.Parse(ConfigurationManager.AppSettings["Reply"]);
			var client = BenchmarkClientFactory.CreateBusClient();
			var bodyGenerator = new BodyGenerator(msgSize);

			if (rpc)
			{
				client.RespondAsync<Message, Message>((message, context) =>
					Task.FromResult(new Message {Body = bodyGenerator.GetNext()})
				);
			}
			else
			{
				client.SubscribeAsync<Message>((message, context) =>
					Task.CompletedTask
				);
			}
			Console.WriteLine($"Server is running with rpc: {rpc}");
		}
	}
}
