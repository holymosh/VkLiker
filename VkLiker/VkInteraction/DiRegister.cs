using Microsoft.Extensions.DependencyInjection;
using VkInteraction.Services.Abstract;
using VkInteraction.Services.Concrete;
using VkNet;
using VkNet.Abstractions;

namespace VkInteraction
{
    public static class DiRegister
    {
        public static void RegisterVkInteraction(this ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IVkService, VkService>();
            serviceCollection.AddSingleton<IVkApi, VkApi>();
        }
    }
}
