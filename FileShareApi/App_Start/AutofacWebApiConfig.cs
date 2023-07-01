using Application.EntityMapper.Contract;
using Application.EntityMapper.Service;
using Application.Persistence;
using Application.Persistence.Common;
using Autofac;
using Autofac.Integration.WebApi;
using Persistence.Contexts;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace FileShareApi.App_Start
{
    public class AutofacWebApiConfig
    {
        public static IContainer Container;

        public static void Initialize(HttpConfiguration config)
        {
            Initialize(config, RegisterServices(new ContainerBuilder()));
        }


        public static void Initialize(HttpConfiguration config, IContainer container)
        {
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static IContainer RegisterServices(ContainerBuilder builder)
        {
            //Register your Web API controllers.  
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            var SQLiteCnn = System.Configuration.ConfigurationManager.ConnectionStrings["SQLiteConnection"].ConnectionString;
            builder.RegisterType(typeof(BaseRepository)).As(typeof(IBaseRepository)).WithParameter("context",
                new FileShareDbContext(new SQLiteConnection()
                {
                    ConnectionString = new SQLiteConnectionStringBuilder()
                    { DataSource = SQLiteCnn, ForeignKeys = true }.ConnectionString
                })).InstancePerRequest();
            builder.RegisterType(typeof(FolderRepository)).As(typeof(IFolderRepository)).WithParameter("context",
                new FileShareDbContext(new SQLiteConnection()
                {
                    ConnectionString = new SQLiteConnectionStringBuilder()
                    { DataSource = SQLiteCnn, ForeignKeys = true }.ConnectionString
                })).InstancePerRequest();
            builder.RegisterType(typeof(UserRepository)).As(typeof(IUserRepository)).WithParameter("context",
                new FileShareDbContext(new SQLiteConnection()
                {
                    ConnectionString = new SQLiteConnectionStringBuilder()
                    { DataSource = SQLiteCnn, ForeignKeys = true }.ConnectionString
                })).InstancePerRequest();
            builder.RegisterType(typeof(EntityMapper)).As(typeof(IEntityMapper)).InstancePerRequest();
            Container = builder.Build();

            return Container;
        }
    }
}