using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Database
{
    public static class DiDbRegister
    {
        public static void RegisterDb(this ServiceCollection serviceCollection)
        {
            var connectionString =
                @"Server=.\SQLExpress;Database=VkLiker;Integrated Security=True;";
            serviceCollection.AddEntityFrameworkSqlServer().AddDbContext<VkContext>(builder => builder.UseSqlServer(connectionString));

        }
    }
}
