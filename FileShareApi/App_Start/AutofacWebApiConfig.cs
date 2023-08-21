using Application.Helper;
using Application.Helpers;
using Application.Persistence;
using Application.Persistence.Common;
using Application.Service.EntityMapper;
using Autofac;
using Autofac.Integration.WebApi;
using FileShareApi.Auth.Contract;
using FileShareApi.Auth.Service;
using FileShareService.EntityMapper;
using FileShareService.Helper;
using Persistence.Contexts;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            var SQLiteCnn = ConfigurationManager.ConnectionStrings["SQLiteConnection"].ConnectionString;
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
            builder.RegisterType(typeof(EncryptionHelper)).As(typeof(IEncryptionHelper)).InstancePerRequest();
            builder.RegisterType(typeof(DirectoryHelper)).As(typeof(IDirectoryHelper)).InstancePerRequest();
            builder.RegisterType(typeof(AccessControl)).As(typeof(IAccessControl)).InstancePerRequest();
            Container = builder.Build();
            return Container;
        }
    }
}