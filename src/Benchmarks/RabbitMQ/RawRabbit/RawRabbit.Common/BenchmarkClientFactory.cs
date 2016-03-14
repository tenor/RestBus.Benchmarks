using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit.Common.CustomImplementations;
using RawRabbit.Logging;
using RawRabbit.vNext;

namespace RawRabbit.Common
{
	public class BenchmarkClientFactory
	{
		public static IBusClient CreateBusClient()
		{
			LogManager.CurrentFactory = new VoidLoggerFactory();

			var collection = new ServiceCollection();
			collection.AddRawRabbit(
				cfg => cfg
					.AddJsonFile("rawrabbit.json"),
				ioc => ioc
					.AddSingleton<IPublishAcknowledger, NoAckAcknowledger>()
			);

			var provider = collection.BuildServiceProvider();
			return provider.GetService<IBusClient>();
		}
	}
}
