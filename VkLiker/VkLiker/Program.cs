using System;
using System.Linq;
using Database;
using Microsoft.Extensions.DependencyInjection;

namespace VkLiker
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.RegisterDb();
            var provider = serviceCollection.BuildServiceProvider();
            var dbcontext = provider.GetService<VkContext>();
            var test = dbcontext.VkCities.FirstOrDefault();
            Console.ReadKey();
        }
    }
}
