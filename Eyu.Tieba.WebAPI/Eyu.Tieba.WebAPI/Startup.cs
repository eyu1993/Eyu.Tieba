using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Eyu.Tieba.WebAPI.Provider;
using Microsoft.Owin.Cors;

[assembly: OwinStartup(typeof(Eyu.Tieba.WebAPI.Startup))]

namespace Eyu.Tieba.WebAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
            //允许跨域获取token
            app.UseCors(CorsOptions.AllowAll);

            app.UseOAuthAuthorizationServer(new Microsoft.Owin.Security.OAuth.OAuthAuthorizationServerOptions()
            {
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                AllowInsecureHttp = true,
                Provider = GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(EYUOAuthAuthorizationServerProvider)) as EYUOAuthAuthorizationServerProvider
            });
            app.UseOAuthBearerAuthentication(new Microsoft.Owin.Security.OAuth.OAuthBearerAuthenticationOptions());
        }
    }
}
