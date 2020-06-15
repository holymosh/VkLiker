using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
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
                            //UploadFriends(tambov),
                            //LikePersonFromDb()
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
                    _logger.Error(e,$"Exception : {e} \n Inner : {e.InnerException}");
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
                    _logger.Error(e, $"Exception : {e} \n Inner : {e.InnerException}");
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
                    _logger.Error(e, $"Exception : {e} \n Inner : {e.InnerException}");
                }

            }
        }

        private long GetItemId(string vkPhotoId) => long.Parse(vkPhotoId.Split('_').Last());
    }
}
