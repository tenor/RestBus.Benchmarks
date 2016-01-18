using System;
using System.Threading;

namespace RestBusTestServer
{
    public static class BodyGenerator
    {
        static byte[][] bodies = new byte[10][];
        static Random rnd = new Random();
        static int next = -1;
        static BodyGenerator()
        {
            //Fill bodies with Random junk
            for (int i = 0; i < 10; i++)
            {
                bodies[i] = new byte[Startup.MessageSize];
                rnd.NextBytes(bodies[i]);
            }
        }

        public static byte[] GetNext()
        {
            var val = Interlocked.Increment(ref next);

            return bodies[val % 10];
        }
    }

}
