//#define BARE_TO_THE_METAL_MODE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestBus.RabbitMQ;
using RestBus.AspNet.Server;
using RestBus.RabbitMQ.Subscription;

namespace RestBusTestServer
{
    public class Startup
    {
        public static int MessageSize { get; set; }

        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            #if !BARE_TO_THE_METAL_MODE
                services.AddMvc();
            #endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            #if !BARE_TO_THE_METAL_MODE
                app.UseMvc();
            #endif

            MessageSize = int.Parse(Configuration["MessageSize"]);

            var amqpUrl = Configuration["ServerUri"]; //AMQP URI for RabbitMQ server
            var serviceName = "speedtest"; //Uniquely identifies this service

            var msgMapper = new BasicMessageMapper(amqpUrl, serviceName);
            var subscriber = new RestBusSubscriber(msgMapper);
            app.ConfigureRestBusServer(subscriber);

            #if BARE_TO_THE_METAL_MODE
                app.Run(async c =>
                {
                    if (c.Request.Path.StartsWithSegments("/api/test"))
                    {
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(new Message { Body = BodyGenerator.GetNext() });
                        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                        await c.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    }
                });
            #endif
        }

        // Entry point for the application.
        public static void Main(string[] args)
        {
            var config = WebApplicationConfiguration.GetDefault(args);

            var application = new WebApplicationBuilder()
                .UseConfiguration(config)
                .UseStartup<Startup>()
                .Build();

            application.Run();

        }
    }
}
