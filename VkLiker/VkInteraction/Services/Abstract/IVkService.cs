using VkNet.Model;
using VkNet.Utils;

namespace VkInteraction.Services.Abstract
{
    public interface IVkService
    {
        VkCollection<City> GetRegionPartsByString(string query);
        VkCollection<Region> GetRegions(string region);
    }
}
