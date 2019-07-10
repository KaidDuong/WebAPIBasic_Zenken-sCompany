using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using SampleAPI.Models;
using SampleAPI.ViewModels;

namespace SampleAPI.Controllers
{
    public class AccountsController : ApiController
    {
        private UserManager<ApplicationUser> userManager;
        private IAuthenticationManager Authentication => Request.GetOwinContext().Authentication;
        public AccountsController()
        {
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new SampleContext()));
        }
        // POST api/Accounts/Register
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Register(RegisterModel model)
        {
            var result = await userManager.CreateAsync(new ApplicationUser
            {
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                UserName = model.UserName
            }, model.Password);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await userManager.FindAsync(model.UserName, model.Password);
                    if (user == null)
                    {
                        return NotFound();
                    }
                    
                    var accesstoken = GenerateLocalAccessTokenResponse(model.UserName?.Trim(), user.Id);

                    return Ok(new
                    {
                        UserInfo = new LoginResult
                        {
                            Id = user.Id,
                            UserName = user.UserName,
                            Email = user.Email,
                        },
                        AccessToken = accesstoken.GetValue("access_token"),
                        ExpiredIn = accesstoken.GetValue("expires_in"),
                        ExpiredDate = DateTime.Parse(accesstoken.GetValue(".expires").ToString())
                    });
                }
                catch (Exception)
                {
                    return BadRequest();
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut();
            return Ok();
        }
        private JObject GenerateLocalAccessTokenResponse(string userName, string id)
        {
            var tokenExpiration = TimeSpan.FromDays(1);

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
            identity.AddClaim(new Claim("role", "user"));

            var props = new AuthenticationProperties
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration)
            };
            var ticket = new AuthenticationTicket(identity, props);
            var accessToken = Startup.OAuthServerOptions.AccessTokenFormat.Protect(ticket);

            JObject tokenResponse = new JObject(
                new JProperty("userName", userName),
                new JProperty("access_token", accessToken),
                new JProperty("token_type", "bearer"),
                new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString(CultureInfo.InvariantCulture)),
                new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
            );
            return tokenResponse;
        }

        public async Task<IHttpActionResult> ChangePassword(ChangePassWordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await userManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
