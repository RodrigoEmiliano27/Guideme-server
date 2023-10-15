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
using System.Reflection;
using GuideMeServerMVC.Utils;

namespace GuideMeServerMVC.Controllers
{

    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly GuidemeDbContext _context;

        public LoginController(IConfiguration configuration, GuidemeDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<IActionResult> Login()
        {

            try
            {
                Debug.WriteLine("Chamou a tela de Login!");
                return View("Login", new UsuarioEstabelecimentoModel());
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }

           
        }

        public async Task<IActionResult> FazLogin([FromForm] UsuarioEstabelecimentoModel usuario)
        {


            try
            {
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
                        return RedirectToAction("Index", "Home");
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
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }


           
        }

       



        

       
    }
}
