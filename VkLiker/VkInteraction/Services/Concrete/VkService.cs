using VkInteraction.Services.Abstract;
using VkNet;
using VkNet.Abstractions;
using VkNet.Enums.Filters;
using VkNet.Model;

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
    }
}
