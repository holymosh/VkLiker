using System.Linq;
using System.Threading.Tasks;
using Common;
using Database;
using Domain.Entities;
using VkInteraction.Services.Abstract;
using VkLiker.Service.Abstract;


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
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.Database.EnsureCreatedAsync();
            var isInitialized = _dbContext.InitOptions.FirstOrDefault();
            VkRegion dbRegion = null;
            if (isInitialized == null)
            {
                var tmbRegion = _vkService.GetRegions("Тамбов").FirstOrDefault();
                if (tmbRegion != null)
                {
                    dbRegion = _dbContext.Set<VkRegion>().Add(new VkRegion()
                    {
                        Title = tmbRegion.Title,
                        SourceId = tmbRegion.Id
                    }).Entity;
                    isInitialized = new ApplicationInitOptions()
                    {
                        IsCitiesSynchronized = true
                    };
                    _dbContext.InitOptions.Add(isInitialized);
                    await _dbContext.SaveChangesAsync();
                }

                var regionParts = TambovCities.ResourceManager.GetString("tambov").Split(',').ToArray(); 
                if (tmbRegion != null)
                {
                    foreach (var part in regionParts)
                    {
                        var parts = _vkService.GetRegionPartsByString(part)
                            .Where(c => c.Region == tmbRegion.Title && c.Id.HasValue)
                            .DistinctBy(c => c.Title)
                            .Select(c => new RegionPart
                            {
                                Title = c.Title,
                                SourceId = c.Id.Value,
                                RegionId = dbRegion.Id,
                                VkRegion = dbRegion,
                            }).ToArray();
                        _dbContext.AddRange(parts);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                    
            }
        }
    }
}
