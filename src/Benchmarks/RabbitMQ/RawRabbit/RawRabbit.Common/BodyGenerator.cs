using System;
using System.Threading;

namespace RawRabbit.Common
{
	public class BodyGenerator
	{
		private readonly byte[][] _bodies;
		private int _next;

		public BodyGenerator(int size)
		{
			var random = new Random();
			_bodies = new byte[10][];
			for (var i = 0; i < 10; i++)
			{
				_bodies[i] = new byte[size];
				random.NextBytes(_bodies[i]);
			}
		}

		public byte[] GetNext()
		{
			var val = Interlocked.Increment(ref _next);

			return _bodies[val % 10];
		}
	}
}
