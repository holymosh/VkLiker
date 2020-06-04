using System;
using System.Collections.Generic;
using System.Text;
using Database;
using VkInteraction.Services.Abstract;
using VkLiker.Service.Abstract;
using VkNet.Abstractions;

namespace VkLiker.Service.Concrete
{
    public class StartupService : IStartupService
    {
        private readonly VkContext _dbContext;
        private readonly IVkService _vkService;

        public StartupService(VkContext dbContext, IVkService vkService)
        {
            _dbContext = dbContext;
            _vkService = vkService;
        }

        public void InitDb()
        {
            var cities = new[] { "Тамбов" };
            foreach (var city in cities)
            {
                Console.WriteLine($"{city} : ");
                var result = _vkService.GetCitiesByString(city);
                foreach (var cityResult in result)
                {
                    Console.Write($"{cityResult.Title}:{cityResult.Region}, ");
                }
                Console.WriteLine();
            }

            Console.ReadKey();
        }
    }
}
