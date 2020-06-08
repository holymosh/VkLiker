using System;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
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
            var tambov = _dbContext.RegionParts.Include(rp => rp.VkRegion).SingleOrDefault(p => p.Title == "Тамбов");
            var offset = 0;
            if (tambov != null)
            {
                var usersFromGlobalSearch = await _vkService.GetUsersFromGlobalSearch((int?)tambov.SourceId,17);
                var arr = usersFromGlobalSearch.ToArray();
            }
            //var holy = await _vkService.GetUser(61802985);
            //await _vkService.SetLike(long.Parse(holy.PhotoId.Split('_').LastOrDefault() ?? throw new InvalidOperationException()), 61802985);

        }

        private async Task LikeGlobalSearchedUsers(RegionPart regionPart)
        {
            var offset = _dbContext.InitOptions.FirstOrDefault().Offset;
            var globalSearchResult = await _vkService.GetUsersFromGlobalSearch((int?)regionPart.SourceId, offset);
            foreach (var currentUser in globalSearchResult)
            {
                try
                {
                    var dbUser = await _dbContext.Users.SingleOrDefaultAsync(u => u.SourceId == currentUser.Id);
                    if (dbUser == null)
                    {
                        await _vkService.SetLike(GetItemId(currentUser.PhotoId), currentUser.Id);
                        var likedUser = new User()
                        {
                            FirstName = currentUser.FirstName,
                            LastName = currentUser.LastName,
                            IsFriendsLiked = false,
                            LikeDate = DateTime.Now,
                            RegionPart = regionPart,
                            IsLiked = true,
                            SourceId = currentUser.Id,
                        };
                        var vkFriends = await _vkService.GetUsers(currentUser.FriendLists);
                        var mappedFriends = vkFriends.Where(u => u.City.Region == regionPart.VkRegion.Title).Select(u =>
                            new User
                            {
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                IsFriendsLiked = false,
                                RegionPart = regionPart,
                                SourceId = u.Id,
                            });
                        foreach (var friend in mappedFriends) likedUser.Friends.Add(friend);
                        await _dbContext.Users.AddRangeAsync(mappedFriends);
                        await _dbContext.Users.AddAsync(likedUser);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private long GetItemId(string vkPhotoId) => long.Parse(vkPhotoId.Split('_').Last());
    }
}
