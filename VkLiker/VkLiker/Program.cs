using System;
using System.Linq;
using Database;
using Microsoft.Extensions.DependencyInjection;
using VkInteraction;
using VkInteraction.Services.Abstract;

namespace VkLiker
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.RegisterDb();
            serviceCollection.RegisterVkInteraction();
            var provider = serviceCollection.BuildServiceProvider();
            InitDb(provider);
            Console.ReadKey();
        }

        static void InitDb(ServiceProvider provider)
        {
            var dbcontext = provider.GetService<VkContext>();
            var test = dbcontext.VkCities.FirstOrDefault();
            var vkSerivce = provider.GetService<IVkService>();
        }
    }
}
