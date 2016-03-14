using RawRabbit.Logging;

namespace RawRabbit.Common.CustomImplementations
{
	public class VoidLoggerFactory : ILoggerFactory
	{
		private static readonly VoidLogger Logger = new VoidLogger();

		public void Dispose()
		{

		}

		public ILogger CreateLogger(string categoryName)
		{
			return Logger;
		}

		public LogLevel MinimumLevel { get; set; }
	}
}