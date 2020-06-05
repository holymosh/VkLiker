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

        public void Start()
        {
        }
    }
}
