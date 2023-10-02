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
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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

        public IActionResult Login()
        {
            Debug.WriteLine("Chamou a tela de Login!");
            return View("Login", new UsuarioEstabelecimentoModel());
        }
        /*public IActionResult LogOff()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index","Home");
        }*/

        [HttpPost("FazLogin")]
        public IActionResult FazLogin([FromForm] UsuarioEstabelecimentoModel usuario)
        {
            System.Diagnostics.Debug.WriteLine("Testei");
            //Valida usuario
            bool isUsernamePasswordValid = false;
            if (usuario != null)
            {
                Debug.WriteLine("Entrou no login");
                //var user = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Login == usuario.Login && o.Senha == usuario.Senha);
                var users = _context.UsuariosEstabelecimento.ToList();
                Debug.WriteLine("users => " + users);
                var user = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Login == usuario.Login);

                isUsernamePasswordValid = user != null ? true : false;

                if (isUsernamePasswordValid)
                {
                    ViewBag.Logado = true;
                    Debug.WriteLine("Logou krai");
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    // Recupere o ID do usuário da sessão
                    var userId = HttpContext.Session.GetInt32("UserId");
                    Debug.WriteLine("userId => " + userId);
                    return RedirectToAction("Index","Home");
                    //return Ok("Login realizado");
                }
                else
                {
                    Debug.WriteLine("N achou");
                    return RedirectToAction("Error");
                    //return NotFound();
                }
            }
            else return RedirectToAction("Error");
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
                return BadRequest(err.ToString());
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
