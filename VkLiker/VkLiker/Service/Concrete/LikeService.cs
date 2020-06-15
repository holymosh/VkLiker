using System;
using System.Data;
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

        private async Task LikeFromGlobalSearch(RegionPart regionPart)
        {
            var appOptions = _dbContext.AppOptions.FirstOrDefault();
            if (appOptions == null) throw new DataException("app options are null");
            var offset = appOptions.Offset;
            var globalSearchResult = await _vkService.GetUsersFromGlobalSearch((int?)regionPart.SourceId, offset);
            foreach (var currentUser in globalSearchResult)
            {
                try
                {
                    var dbUser = await _dbContext.Users.SingleOrDefaultAsync(u => u.SourceId == currentUser.Id);
                    if (dbUser == null)
                    {
                        await _vkService.SetLike(GetItemId(currentUser.PhotoId), currentUser.Id);
                        var likedUser = new User
                        {
                            FirstName = currentUser.FirstName,
                            LastName = currentUser.LastName,
                            LikeDate = DateTime.Now,
                            RegionPart = regionPart,
                            IsLiked = true,
                            IsFriendsUploaded = false,
                            SourceId = currentUser.Id
                        };
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

            appOptions.Offset += (uint) globalSearchResult.Count();
            _dbContext.AppOptions.Update(appOptions);
            await _dbContext.SaveChangesAsync();
        }

        private async Task UploadFriends(RegionPart regionPart)
        {
            var currentUser = await _dbContext.Users.FirstOrDefaultAsync(u => !u.IsFriendsUploaded);
            if (currentUser != null)
            {
                try
                {
                    var userInfo = await _vkService.GetUser(currentUser.SourceId);
                    var friendIds = userInfo.FriendLists;
                    var friends = await _vkService.GetUsers(friendIds);
                    var newFriends = friends.Where(u => u.City.Region == regionPart.VkRegion.Title).Select(u =>
                        new User
                        {
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            RegionPart = regionPart,
                            SourceId = u.Id,
                            IsFriendsUploaded = false,
                            IsLiked = false
                        });
                    await _dbContext.AddRangeAsync(newFriends);
                    foreach (var friend in newFriends) currentUser.Friends.Add(friend);
                    _dbContext.Users.Update(currentUser);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    
                }
            }
        }

        private async Task LikePersonFromDb()
        {
            var currentUser = await _dbContext.Users.FirstOrDefaultAsync(u => !u.IsLiked);
            if (currentUser != null)
            {
                try
                {
                    var userInfo = await _vkService.GetUser(currentUser.SourceId);
                    await _vkService.SetLike(GetItemId(userInfo.PhotoId), currentUser.Id);
                    currentUser.IsLiked = true;
                    _dbContext.Users.Update(currentUser);
                    await _dbContext.SaveChangesAsync();
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
