using Eyu.Tieba.Common;
using Eyu.Tieba.UnitOfWork;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Owin;

namespace Eyu.Tieba.WebAPI.Provider
{
    public class EYUOAuthAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public IUnitOfWork UOW { get; set; }
        public EYUOAuthAuthorizationServerProvider(IUnitOfWork unitOfWork)
        {
            this.UOW = unitOfWork;
        }

        //验证客户端
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return base.ValidateClientAuthentication(context);
        }

        //根据用户名和密码生成token
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            string password = MD5Helper.GetStrMD5(context.Password);
            string account = context.UserName;
            int number = RedisHelper.GetValueFromMyHash<int>(Operation.Login, account);
            if (number >= 5)
            {
                context.SetError("验证失败", "账号被锁定，请30分钟后再试");
                LogHelper.Info(string.Format("User {0} login failed. IP address is {1}. Reason: Account is locked because too many password error.", account, context.Request.RemoteIpAddress));
                return;
            }

            var user = await UOW.UserRepository.GetSingleAsync(u => (u.Name == account || u.Phone == account) && u.Password == password && u.IsActive == true);
            if (user == null)
            {
                RedisHelper.SetEntryInMyHash<int>(Operation.Login, account, number + 1, DateTime.Now.AddMinutes(30));
                context.SetError("验证失败", "用户名或密码错误");
                LogHelper.Info(string.Format("User {0} login failed. IP address is {1}. Reason: User name or password error.", account, context.Request.RemoteIpAddress));
                return;
            }

            RedisHelper.RemoveEntryFromMyHash(Operation.Login, account);
            LogHelper.Info(string.Format("User {0} login successful. IP address is {1}.", user.Name, context.Request.RemoteIpAddress));
            ClaimsIdentity identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
            AuthenticationTicket ticket = new AuthenticationTicket(identity, CreateProperties(user.Name));
            context.Validated(ticket);
        }

        //将property随token一起返回到客户端
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return base.TokenEndpoint(context);
        }

        //生成property
        private static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>();
            data.Add("userName", userName);
            return new AuthenticationProperties(data);
        }
    }
}