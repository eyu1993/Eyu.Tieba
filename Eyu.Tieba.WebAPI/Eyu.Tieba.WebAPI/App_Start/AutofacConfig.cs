using Autofac;
using Autofac.Integration.WebApi;
using Eyu.Tieba.WebAPI.Provider;
using System.Reflection;
using System.Web.Http;

namespace Eyu.Tieba.WebAPI
{
    public class AutofacConfig
    {
        public static void Register()
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<UnitOfWork.UnitOfWork>().As<UnitOfWork.IUnitOfWork>();
            builder.RegisterType<EYUOAuthAuthorizationServerProvider>();
            var container = builder.Build();
            var config = GlobalConfiguration.Configuration;
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}