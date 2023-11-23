using GuideMeServerMVC.Data;
using GuideMeServerMVC.Models;
using GuideMeServerMVC.TO;
using GuideMeServerMVC.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;

namespace GuideMeServerMVC.Controllers
{
    [Route("api/Login")]
    [ApiController]
    public class LoginAPIController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly GuidemeDbContext _context;

        public LoginAPIController(IConfiguration configuration, GuidemeDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        //https://localhost:7048/api/Login/login
        [HttpPost("v1/login")]
        [AllowAnonymous]
        public ActionResult<object> Authenticate([FromBody] LoginRequestTO login)
        {
            try
            {
                var loginResponse = new LoginResponseTO { };
                LoginRequestTO loginrequest = new()
                {
                    UserName = login.UserName.ToLower(),
                    Password = login.Password
                };

                bool isUsernamePasswordValid = false;
                bool isUsuarioApp = false;
                bool isUsuarioEstabelecimento = false;
                int id = 0;
                UsuarioEstabelecimentoModel usuarioEstabelecimento = null;
                if (login != null)
                {

                    var usuarioApp = _context.AppLogin.AsNoTracking().FirstOrDefault(o => o.Login == login.UserName && o.Senha == login.Password);
                    if (usuarioApp == null)
                        usuarioEstabelecimento = _context.UsuariosEstabelecimento.AsNoTracking().FirstOrDefault(o => o.Login == login.UserName && o.Senha == login.Password);
                    // make await call to the Database to check username and password.
                    // here we only checking if password value is admin
                    if (usuarioApp != null)
                        id = usuarioApp.Id;
                    else if (usuarioEstabelecimento != null)
                        id = usuarioEstabelecimento.Id;


                    isUsuarioApp = usuarioApp != null ? true : false;
                    isUsernamePasswordValid = (usuarioApp != null || usuarioEstabelecimento != null) ? true : false;
                }
                // if credentials are valid
                if (isUsernamePasswordValid)
                {
                    string token = CreateToken(loginrequest.UserName, id, isUsuarioApp, usuarioEstabelecimento);

                    loginResponse.Token = token;


                    //return the token
                    return Ok(new { loginResponse });
                }
                else
                {
                    // if username/password are not valid send unauthorized status code in response               
                    return BadRequest("Username or Password Invalid!");
                }
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err,
                    naoProcureEstabelecimento: true);
                return Unauthorized("Username or Password Invalid!");
            }
        }
        private string CreateToken(string username, int id, bool app = false, UsuarioEstabelecimentoModel usuarioEstabelecimento = null)
        {

            List<Claim> claims = new()
            {                    
                //list of Claims - we only checking username - more claims can be added.
                new Claim("username", Convert.ToString(username)),
                new Claim("id", id.ToString())
            };
            if (app)
                claims.Add(new Claim(ClaimTypes.Role, "app"));
            else if (usuarioEstabelecimento != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, "estabelecimento"));
                claims.Add(new Claim("IdEstabelecimento", Convert.ToString(usuarioEstabelecimento.Id_Estabelecimento)));
            }


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
