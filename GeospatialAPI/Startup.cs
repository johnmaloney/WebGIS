using GeospatialAPI.Controllers;
using GeospatialAPI.ServiceBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace GeospatialAPI
{
    public class Startup
    {
        const string ServiceBusConnectionString = "Endpoint=sb://aetosmessaging.servicebus.windows.net/;SharedAccessKeyName=Publisher;SharedAccessKey=knJ9TZyB9kf8kdv/cCcTW4b9/sPCTP5tcX2G9zU1QUE=";
        const string TileListenerConnectionString = "Endpoint=sb://aetosmessaging.servicebus.windows.net/;SharedAccessKeyName=Admin;SharedAccessKey=cRU4D8IA7y7sfywt0t9b6hksWaTyFBkqY67+KxcUzEY=;EntityPath=tile-query";
        const string QueueName = "aetosMessaging";
        readonly string appType;

        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            appType = config.GetSection("appType").Value;
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            //var serviceProvider = services.BuildServiceProvider();
            //Configuration = serviceProvider.GetService<ConfigurationRoot>();
            
            services.AddMvc();
            services.AddOptions();

            services.AddSingleton<IQueueObserverClient>(
                new QueueObserverClient(
                    new QueueClientOptions
                    {
                        ConnectionString = ServiceBusConnectionString,
                        AppType =appType
                    }));
                    
            services.AddSingleton<ITileQueueClient>(
                new TileQueueClient(
                    new QueueClientOptions
                    {
                        ConnectionString = ServiceBusConnectionString,
                        AppType = appType
                    }));
            services.AddSingleton<IAnalysisQueueClient>(
                new AnalysisQueueClient(
                    new QueueClientOptions
                    {
                        ConnectionString = ServiceBusConnectionString,
                        AppType = appType
                    }));
        }

        public void Configure(IApplicationBuilder app)
        { 
            app.UseMvc();
        }
    }
    
}
