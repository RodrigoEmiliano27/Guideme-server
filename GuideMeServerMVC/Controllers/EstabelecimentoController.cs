using GuideMeServerMVC.Data;
using GuideMeServerMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using GuideMeServerMVC.Utils;
using System.Reflection;

namespace GuideMeServerMVC.Controllers
{
    public class EstabelecimentoController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly GuidemeDbContext _context;

        public EstabelecimentoController(IConfiguration configuration, GuidemeDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
       
        public async Task<IActionResult> CadastrarEstabelecimento()
        {
            Debug.WriteLine("Chamou a tela de Cadastro de Estabelecimento!");
            return View("CadastroEstabelecimento", new EstabelecimentoViewModel());
        }
        public async Task<IActionResult> Create([FromForm] EstabelecimentoViewModel estabelecimento) //Create e Update
        {
            try
            {
                _context.Database.BeginTransaction();
                Debug.WriteLine("estabelecimento -> " + estabelecimento.Id);
                if (estabelecimento.Id == 0) //Create
                {
                    Debug.WriteLine("CreateEstabelecimento");
                    //Cadastrar o estabelecimento
                    _context.Estabelecimento.Add(estabelecimento);
                    _context.SaveChanges();
                    Debug.WriteLine("Estabelecimento criado");

                    //Pegar o id do estabelecimento inserido e relaciona-lo ao usuario
                    Debug.WriteLine("Estabelecimento Id => " + estabelecimento.Id);
                    Debug.WriteLine("HttpContext.Session.GetInt32(UserId) => " + HttpContext.Session.GetInt32("UserId"));
                    var user = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Id == HttpContext.Session.GetInt32("UserId"));
                    Debug.WriteLine("user => " + (user != null));
                    if (user != null)
                    {
                        user.Id_Estabelecimento = estabelecimento.Id;
                        _context.Update(user);
                        _context.SaveChanges();
                        _context.Database.CommitTransaction();
                        Debug.WriteLine("funcionou?");
                        return RedirectToAction("Index", "UsuarioEstabelecimento");
                    }
                    else
                    {
                        return View(new ErrorViewModel { RequestId = "Usuário não encontrado" });
                    }
                }
                else  //Update
                {
                    Debug.WriteLine("Editar Estabelecimento");
                    _context.Estabelecimento.Update(estabelecimento);
                    _context.SaveChanges();
                    _context.Database.CommitTransaction();
                    Debug.WriteLine("Estabelecimento Atualizado");
                }
                return RedirectToAction("Index", "UsuarioEstabelecimento");
            }
            catch (Exception ex)
            {
                // Em caso de erro, desfaz todas as alterações no banco de dados
                _context.Database.RollbackTransaction();

                Debug.WriteLine("Erro durante a operação: " + ex.Message);
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex,
                    naoProcureEstabelecimento: true);
                return View(new ErrorViewModel { RequestId = ex.Message });
            }
            
        }
        public async Task<IActionResult> EditarEstabelecimento()  //Chama a tela de editar
        {
            UsuarioEstabelecimentoModel user = null;
            try
            {
                Debug.WriteLine("Chamou a tela de Editar Estabelecimento!");
                user = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Id == HttpContext.Session.GetInt32("UserId"));
                if (user != null)
                {
                    var estab = _context.Estabelecimento.FirstOrDefault(o => o.Id == user.Id_Estabelecimento);
                    if (estab != null && user.Id_Estabelecimento!=null)
                    {
                        EstabelecimentoViewModel estabelecimento = new EstabelecimentoViewModel();
                        estabelecimento.Id = (int)user.Id_Estabelecimento;
                        estabelecimento.Nome = estab.Nome;

                        return View("CadastroEstabelecimento", estabelecimento);
                    }
                    else
                    {
                        return View(new ErrorViewModel { RequestId = "Estabelecimento não encontrado" });
                    }
                }
                else
                {
                    Debug.WriteLine("Usuário não encontrado");
                    Debug.WriteLine("user -> " + user);
                    return View(new ErrorViewModel { RequestId = "Usuário não encontrado" });
                }
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err,
                    EstabelecimentoID: user!=null ?user.Id_Estabelecimento:null);

                return View(new ErrorViewModel { RequestId = "Usuário não encontrado" });
            }
        
        }
    }
}
