using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Server.Controllers
{
	[EnableCors(origins: "*", headers: "*", methods: "*")]
	public class AccountController : ApiController
	{
		WolfProjectsEntities db = new WolfProjectsEntities();

		[HttpGet]
		[Route("logout")]
		public WebResult<string> logout([UserLogged] User user)
		{
			//get user data
			var identity = User.Identity as ClaimsIdentity;
			var claim = identity.Claims.Where(c => c.Type == ClaimTypes.Name).SingleOrDefault();
			//remove user data
			identity.RemoveClaim(claim);
			return new WebResult<string>
			{
				Success = true,
				Message = $"{user.name} התנתק בהצלחה",
				Value = null
			};
		}

		[HttpGet]
		[Route("login")]
		public async Task<WebResult<LoginData>> Login(string username, string password)
		{
			var user = db.Users.Where(w => w.name == username && w.password == password).FirstOrDefault();
			if (user != null)//אם המשתמש קיים במאגר המשך לקבלת טוקן, אחרת החזר שגיאה שהמתשמש לא קיים
			{
				var accessToken = await GetTokenDataAsync(username, password);

				if (!string.IsNullOrEmpty(accessToken))
				{
					return new WebResult<LoginData>
					{
						Success = true,
						Message = "התחברת בהצלחה",
						Value = new LoginData
						{
							TokenJson = accessToken,
							User = user
						}
					};
				}
			}
			return new WebResult<LoginData>
			{
				Success = false,
				Message = "אין משתמש רשום בשם וסיסמא זו....",
				Value = null
			};

		}

		[HttpPost]
		[Route("register")]
		public async Task<WebResult<LoginData>> Register(User user)
		{
			db.Users.Add(user);
			if (db.SaveChanges() > 0)//בדיקה שהמידע נשמר
			{
				var accessToken = await GetTokenDataAsync(user.name, user.password);

				if (!string.IsNullOrEmpty(accessToken))
				{
					return new WebResult<LoginData>
					{
						Success = true,
						Message = "התחברת בהצלחה",
						Value = new LoginData
						{
							TokenJson = accessToken,
							User = user
						}
					};

				}
			}
			return new WebResult<LoginData>
			{
				Success = false,
				Message = "אין משתמש רשום בשם וסיסמא זו....",
				Value = null
			};

		}

		private async Task<string> GetTokenDataAsync(string username, string password)
		{
			HttpClient httpClient = new HttpClient();
			httpClient.BaseAddress = new Uri(Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + "/token");
			var postData = new List<KeyValuePair<string, string>>();
			postData.Add(new KeyValuePair<string, string>("UserName", username));
			postData.Add(new KeyValuePair<string, string>("Password", password));
			postData.Add(new KeyValuePair<string, string>("grant_type", "password"));//don't dare to change that!!!
			HttpContent content = new FormUrlEncodedContent(postData);
			HttpResponseMessage response = await httpClient.PostAsync("token", content);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}
	}
}
