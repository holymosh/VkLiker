using System;
using System.Collections.Generic;
using System.Linq;
using VkInteraction.Services.Abstract;
using VkNet.Abstractions;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams.Database;
using VkNet.Utils;

namespace VkInteraction.Services.Concrete
{
    public class VkService : IVkService
    {
        private readonly IVkApi _vkApi;

        public VkService(IVkApi vkApi)
        {
            _vkApi = vkApi;
            Authorize();
        }

        private void Authorize()
        {
            var authParams = new ApiAuthParams()
            {
                ApplicationId = VkSettings.AppId,
                Login = VkSettings.Login,
                Password = VkSettings.Password,
                Settings = Settings.All
            };
            _vkApi.Authorize(authParams);
        }

        public VkCollection<City> GetCitiesByString(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException($"city can't be null");
            }
            return _vkApi.Database.GetCities(new GetCitiesParams
            {
                CountryId = 1,
                NeedAll = true,
                Query = query
            });
        }
    }
}
