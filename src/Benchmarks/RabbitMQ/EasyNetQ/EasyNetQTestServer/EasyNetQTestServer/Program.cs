using EasyNetQ;
using EasyNetQTestCommon;
using System;
using System.Configuration;

namespace EasyNetQTestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            bool reply = bool.Parse(ConfigurationManager.AppSettings["Reply"]);

            var bus = RabbitHutch.CreateBus(ConfigurationManager.AppSettings["ServerConnectionString"]);

            if (reply)
            {
                bus.Respond<Message, Message>(request => new Message { Body = BodyGenerator.GetNext() });
            }
            else
            {
                bus.Receive<Message>("easynetq_queue", m => { /*Do nothing */ });
            }

            Console.WriteLine("Server Started ... Ctrl-C to exit.");
        }
    }

}
