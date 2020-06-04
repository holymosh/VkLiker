using System;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using VkInteraction;
using VkInteraction.Services.Abstract;
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
            Console.ReadKey();
        }


    }

    static class DiRegister
    {
        public static void RegisterMainModule(this ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IStartupService, StartupService>();
        }
    }
}
