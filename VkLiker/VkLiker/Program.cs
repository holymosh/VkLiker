using System;
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
            Console.ReadKey();
        }
    }
}
