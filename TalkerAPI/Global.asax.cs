using ServiceStack.Configuration;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.WebHost.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TalkerAPI.API;

namespace TalkerAPI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            new TalkerAPIAppHost().Init();
        }
    }

    public class TalkerAPIAppHost : AppHostBase
    {
        public TalkerAPIAppHost() : base("Talker Web Services", typeof(UserService).Assembly) { }

        public override void Configure(Funq.Container container)
        {
            SetConfig(new EndpointHostConfig { ServiceStackHandlerFactoryPath = "api" });
            Plugins.Add(new RegistrationFeature());
            var appset = new AppSettings();
            var connstring = appset.Get("SQLSERVER_CONNECTION_STRING", ConfigUtils.GetConnectionString("TalkerConnection"));

            container.Register<IDbConnectionFactory>(c => new OrmLiteConnectionFactory(connstring,
                    SqlServerOrmLiteDialectProvider.Instance));
            container.Register<ServiceStack.CacheAccess.ICacheClient>(c => new ServiceStack.CacheAccess.Providers.MemoryCacheClient());

            Plugins.Add(new AuthFeature(() => new AuthUserSession(), new IAuthProvider[] { 
                new BasicAuthProvider(),
                new CredentialsAuthProvider(appset)
            }) { HtmlRedirect = null });
            container.Register<IUserAuthRepository>(c => new OrmLiteAuthRepository(container.Resolve<IDbConnectionFactory>()));

            var dbFactory = container.Resolve<IDbConnectionFactory>();
            var authRepo = (OrmLiteAuthRepository)container.Resolve<IUserAuthRepository>();
            if (appset.Get("RecreateAuthTables", true))
            {
                authRepo.DropAndReCreateTables();
            }
            else
                authRepo.CreateMissingTables();

            dbFactory.Run(db => db.CreateTable<API.Record>(overwrite: true));

        }
    }
}