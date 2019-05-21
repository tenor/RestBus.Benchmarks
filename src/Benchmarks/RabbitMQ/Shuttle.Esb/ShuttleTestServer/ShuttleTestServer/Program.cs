using System;
using Ninject;
using Shuttle.Core.Ninject;
using Shuttle.Esb;

namespace MassTransitTestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new NinjectComponentContainer(new StandardKernel());

            ServiceBus.Register(container);

            using (ServiceBus.Create(container).Start())
            {
                Console.WriteLine("Server started... Ctrl+C to exit.");
                Console.ReadKey();
            }
        }
    }
}
