using System;
using System.Linq;
using System.Threading.Tasks;
using Database;
using VkInteraction.Services.Abstract;
using VkLiker.Service.Abstract;

namespace VkLiker.Service.Concrete
{
    public class LikeService : ILikeService
    {
        private readonly VkContext _dbContext;
        private readonly IVkService _vkService;

        public LikeService(VkContext dbContext, IVkService vkService)
        {
            _dbContext = dbContext;
            _vkService = vkService;
        }

        public async Task Start()
        {
            var tambov = _dbContext.RegionParts.SingleOrDefault(p => p.Title == "Тамбов");
            //if (tambov != null)
            //{
            //    var usersFromGlobalSearch = await _vkService.GetUsersFromGlobalSearch((int?) tambov.SourceId);
            //    _vkService.SetLike()
            //}
            var holy = await _vkService.GetUser(61802985);
            await _vkService.SetLike(long.Parse(holy.PhotoId.Split('_').LastOrDefault() ?? throw new InvalidOperationException()), 61802985);
        }
    }
}
