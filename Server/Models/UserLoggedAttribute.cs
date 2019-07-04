using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;

namespace Server.Models
{
	public class UserLoggedAttribute : ParameterBindingAttribute
	{
		WolfProjectsEntities db = new WolfProjectsEntities();
		public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter)
		{
			return new UserLoggedParameterBinding(parameter);
		}
	}

	public class UserLoggedParameterBinding : HttpParameterBinding
	{
		WolfProjectsEntities db = new WolfProjectsEntities();
		public UserLoggedParameterBinding(HttpParameterDescriptor parameter)
			: base(parameter)
		{
		}

		public override Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider,
		HttpActionContext actionContext, CancellationToken cancellationToken)
		{
			var user = HttpContext.Current.User as ClaimsPrincipal;
			var identity = user.Identity as ClaimsIdentity;
			var claim = identity.Claims.Where(c => c.Type == ClaimTypes.Name).Select(s=>s.Value).SingleOrDefault();
			actionContext.ActionArguments[Descriptor.ParameterName] = db.Users.Where(w => w.name == claim).FirstOrDefault();
			return Task.FromResult<object>(null);
		}
	}


}