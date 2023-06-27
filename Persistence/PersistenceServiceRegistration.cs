using Application.Persistence;
using Application.Persistence.Common;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection ConfigurePersistenceService(this IServiceCollection services)
        {
            services.AddScoped<DbContext,FileShareDbContext>();
            services.AddScoped<IBaseRepository, BaseRepository>();
            services.AddScoped<IFolderRepository, FolderRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}
