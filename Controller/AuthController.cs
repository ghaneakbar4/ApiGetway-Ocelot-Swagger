using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APIGetway.Controller
{
    [Route("api/v1/auth/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet("/login")]
        [Authorize]
        public IActionResult  login()
        {
            var accessToken = HttpContext.GetTokenAsync("AccessToken");
            var res = accessToken.Result;
            Dictionary<string,string> data = new Dictionary<string,string>();
            foreach (var u in User.Claims)

            {

                data.Add(u.Type, u.Value);

            }
            return Ok(data);    
        }
    }
}
