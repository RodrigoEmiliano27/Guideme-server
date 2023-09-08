using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using GuideMeServerMVC.Models;

namespace GuideMeServerMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View("Index", new LoginRequest());
        }

        //https://localhost:7048/api/Login/login
        [HttpPost("login")]
        public ActionResult<object> Authenticate([FromBody] LoginRequest login)
        {
            var loginResponse = new LoginResponse { };
            LoginRequest loginrequest = new()
            {
                UserName = login.UserName.ToLower(),
                Password = login.Password
            };

            bool isUsernamePasswordValid = false;

            if (login != null)
            {
                // make await call to the Database to check username and password.
                // here we only checking if password value is admin
                isUsernamePasswordValid = loginrequest.Password == "admin" ? true : false;
            }
            // if credentials are valid
            if (isUsernamePasswordValid)
            {
                string token = CreateToken(loginrequest.UserName);

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

        [HttpPost("fazLogin")]
        public async Task<ActionResult<dynamic>> FazLogin([FromForm] LoginRequest model)
        {
            try
            {
                bool isUsernamePasswordValid = false;
                string token = "";
                if (model.UserName != null)
                {
                    // make await call to the Database to check username and password.
                    // here we only checking if password value is admin
                    isUsernamePasswordValid = model.Password == "admin" ? true : false;
                    token = CreateToken(model.UserName);
                }

                /*HttpContext.Session.SetString("Logado", db_user.id.ToString());
                HttpContext.Session.SetString("idPlant", db_user.id_plantacao.ToString());
                HttpContext.Session.SetString("Name", db_user.Nome);
                HttpContext.Session.SetString("Tipo", db_user.Tipo.ToString());
                HttpContext.Session.SetString("Token", token);
                return RedirectToAction("index", "Home");*/
                System.Diagnostics.Debug.WriteLine(token);
                return RedirectToAction("index", "Home");
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }

        }
        private string CreateToken(string username)
        {

            List<Claim> claims = new()
            {                    
                //list of Claims - we only checking username - more claims can be added.
                new Claim("username", Convert.ToString(username)),
            };

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
