using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VkLiker.Service.Abstract
{
    public interface IStartupService
    {
        Task InitDb();
        Task Start();
    }
}
