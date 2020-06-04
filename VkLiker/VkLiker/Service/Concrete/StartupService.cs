using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database;
using Domain.Entities;
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

        public async Task InitDb()
        {
            var tmb = _vkService.GetRegions("Тамбов").FirstOrDefault();
            if (tmb != null)
            {
                _dbContext.Set<Region>().Add(new Region()
                    {
                        Title = tmb.Title,
                        SourceId = tmb.Id
                    });
                await _dbContext.SaveChangesAsync();
            }

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
