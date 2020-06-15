using System;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using VkInteraction;
using VkLiker.Service.Abstract;
using VkLiker.Service.Concrete;

namespace VkLiker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            
            var serviceCollection = new ServiceCollection();
            serviceCollection.RegisterDb();
            serviceCollection.RegisterVkInteraction();
            serviceCollection.RegisterDomain();
            serviceCollection.RegisterMainModule();
            var provider = serviceCollection.BuildServiceProvider();
            var startupService = provider.GetService<IStartupService>();
            await startupService.InitDb();
            startupService.Start();
            Console.ReadKey();
        }
    }

    static class DiRegister
    {
        public static void RegisterMainModule(this ServiceCollection serviceCollection)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs\\myapp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            var logger = Log.Logger;
            logger.Information("App initialization");
            serviceCollection.AddSingleton(logger); // ILogger
            serviceCollection.AddSingleton<IStartupService, StartupService>();
            serviceCollection.AddSingleton<ILikeService, LikeService>();
        }
    }
}
