using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using GuideMeServerMVC.Models;
using GuideMeServerMVC.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using GuideMeServerMVC.TO;

namespace GuideMeServerMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly GuidemeDbContext _context;

        public LoginController(IConfiguration configuration, GuidemeDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public IActionResult Index()
        {
            return View("Index", new LoginRequestTO());
        }

        //https://localhost:7048/api/Login/login
        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult<object> Authenticate([FromBody] LoginRequestTO login)
        {
            var loginResponse = new LoginResponseTO { };
            LoginRequestTO loginrequest = new()
            {
                UserName = login.UserName.ToLower(),
                Password = login.Password
            };

            bool isUsernamePasswordValid = false;
            bool isUsuarioApp = false;
            int id = 0;

            if (login != null)
            {
                var usuarioApp = _context.AppLogin.FirstOrDefault(o => o.Login == login.UserName && o.Senha == login.Password);
                // make await call to the Database to check username and password.
                // here we only checking if password value is admin
                id = usuarioApp.Id;
                isUsuarioApp= usuarioApp != null ? true : false;
                isUsernamePasswordValid = usuarioApp != null? true : false;
            }
            // if credentials are valid
            if (isUsernamePasswordValid)
            {
                string token = CreateToken(loginrequest.UserName,id, isUsuarioApp);

                loginResponse.Token = token;
                loginResponse.responseMsg = new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK
                };

                //return the token
                return Ok(new { loginResponse });
            }
            else
            {
                // if username/password are not valid send unauthorized status code in response               
                return BadRequest("Username or Password Invalid!");
            }
        }
        
        private string CreateToken(string username,int id,bool app=false)
        {

            List<Claim> claims = new()
            {                    
                //list of Claims - we only checking username - more claims can be added.
                new Claim("username", Convert.ToString(username)),
                new Claim("id", id.ToString())
            };
            if (app)
                claims.Add(new Claim(ClaimTypes.Role, "app"));

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: cred
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
