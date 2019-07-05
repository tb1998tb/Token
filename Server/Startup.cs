using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Owin;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

[assembly: OwinStartup(typeof(Server.Startup))]
namespace Server
{
	public class Startup
	{
		[EnableCors(origins: "*", headers: "*", methods: "*")]
		public void Configuration(IAppBuilder app)
		{

			app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

			var myProvider = new MyAuthProvider();
			OAuthAuthorizationServerOptions options = new OAuthAuthorizationServerOptions
			{
				AllowInsecureHttp = true,
				TokenEndpointPath = new PathString("/token"),
				AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(180),
				Provider = myProvider
			};
			app.UseOAuthAuthorizationServer(options);
			app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());


			HttpConfiguration config = new HttpConfiguration();
			WebApiConfig.Register(config);
		}
	}

	public class MyAuthProvider : OAuthAuthorizationServerProvider
	{

		public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
		{
			context.Validated();
			return Task.FromResult<object>(null);

		}

		public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			if (HttpContext.Current.Request.UrlReferrer == null&&HttpContext.Current.Request.UserAgent==null)
			{
				var dict = new Dictionary<string, string>();
				var identity = new ClaimsIdentity(context.Options.AuthenticationType);
				identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
				context.Validated(identity);
			}
			else context.Rejected();
			return Task.FromResult<object>(null);
		}
	}

}
