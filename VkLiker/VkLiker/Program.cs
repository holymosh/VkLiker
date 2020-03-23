using System;
using Microsoft.Extensions.DependencyInjection;

namespace VkLiker
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection();
            
            var provider = serviceProvider.BuildServiceProvider();
        }
    }
}
