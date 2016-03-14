using System;
using RawRabbit.Logging;

namespace RawRabbit.Common.CustomImplementations
{
	public class VoidLogger : ILogger
	{
		public void LogDebug(string format, params object[] args)
		{
		}

		public void LogInformation(string format, params object[] args)
		{
		}

		public void LogWarning(string format, params object[] args)
		{
		}

		public void LogError(string message, Exception exception)
		{
		}
	}
}
