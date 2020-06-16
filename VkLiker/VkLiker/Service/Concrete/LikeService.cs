using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;
using VkInteraction.Services.Abstract;
using VkLiker.Service.Abstract;

namespace VkLiker.Service.Concrete
{
    public class LikeService : ILikeService
    {
        private readonly VkContext _dbContext;
        private readonly IVkService _vkService;
        private readonly ILogger _logger;

        public LikeService(VkContext dbContext, IVkService vkService, ILogger logger)
        {
            _dbContext = dbContext;
            _vkService = vkService;
            _logger = logger;
        }

        public void Start()
        {
            var tambov = _dbContext.RegionParts.Include(rp => rp.VkRegion).SingleOrDefault(p => p.Title == "Тамбов");
            if (tambov != null)
            {
                while (true)
                {
                    try
                    {
                        var tasks = new[]
                        {
                            LikeFromGlobalSearch(tambov),
                            UploadFriends(tambov),
                            LikePersonsFromDb()
                        };
                        Task.WaitAll(tasks);
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e, $"Exception : {e} \n Inner : {e.InnerException}");
                    }
                }
            }

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
                    var userInfo = await _vkService.GetUser(currentUser.Id);
                    if (dbUser == null)
                    {
                        await _vkService.SetLike(GetItemId(userInfo.PhotoId), currentUser.Id);
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
                    _logger.Error(e,$"Exception : {e} \n Inner : {e.InnerException}");
                }
            }

            appOptions.Offset += (uint) globalSearchResult.Count();
            _dbContext.AppOptions.Update(appOptions);
            await _dbContext.SaveChangesAsync();
        }

        private async Task UploadFriends(RegionPart regionPart)
        {
            var currentUser = await _dbContext.Users.Include(u => u.Friends).FirstOrDefaultAsync(u => !u.IsFriendsUploaded);
            if (currentUser != null)
            {
                try
                {
                    var offset = 0;
                    var getNext = true;
                    var cities = TambovCities.ResourceManager.GetString("tambov").Split(',');
                    while (getNext)
                    {
                        var friends = await _vkService.GetUserFriends(currentUser.SourceId, offset);
                        if (friends.Count > 0)
                        {
                            var filteredFriends = friends.Where(u => u.City != null).Where(u => cities.Contains(u.City.Title) || u.City?.Region != null &&
                                                                u.City.Region == regionPart.VkRegion.Title).Select(u =>
                                new User
                                {
                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    RegionPart = regionPart,
                                    SourceId = u.Id,
                                    IsFriendsUploaded = false,
                                    IsLiked = false
                                }).ToArray();
                            if (filteredFriends.Length > 0)
                            {
                                await _dbContext.AddRangeAsync(filteredFriends);
                                foreach (var friend in filteredFriends) currentUser.Friends.Add(friend);
                                _dbContext.Users.Update(currentUser);
                                await _dbContext.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            getNext = false;
                        }

                        offset += 100;

                    }

                    currentUser.IsFriendsUploaded = true;
                    _dbContext.Update(currentUser);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.Error(e, $"Exception : {e} \n Inner : {e.InnerException}");
                }
            }
        }

        private async Task LikePersonsFromDb()
        {
            var users = await _dbContext.Users.Where(u => !u.IsLiked).Take(30).ToArrayAsync();
            if (users.Length > 0)
            {
                try
                {
                    foreach (var user in users)
                    {
                        var userInfo = await _vkService.GetUser(user.SourceId);
                        await _vkService.SetLike(GetItemId(userInfo.PhotoId), userInfo.Id);
                        user.IsLiked = true;
                        _dbContext.Users.Update(user);
                    }
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.Error(e, $"Exception : {e} \n Inner : {e.InnerException}");
                }
            }
        }

        private long GetItemId(string vkPhotoId) => long.Parse(vkPhotoId.Split('_').Last());
    }
}
