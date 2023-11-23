using GuideMeServerMVC.Data;
using GuideMeServerMVC.Enum;
using GuideMeServerMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GuideMeServerMVC.Utils
{
    public class HelperControllers
    {
        public static Boolean VerificaUserLogado(ISession session)
        {
            int? logado = session.GetInt32("UserId");
            if (logado != null && logado >0)
                return true;
            else
                return false;
        }

        public static async Task LoggerErro(ISession session,GuidemeDbContext _context,string controller,string metodo, Exception erro, string Estabelecimento=null, 
            int? EstabelecimentoID=null,bool naoProcureEstabelecimento=false)
        {
            try
            {
                int idUsuarioLogado = GetUserLogadoID(session);
                int idEstabelecimento = 0;
                string NomeEstabelecimento = Estabelecimento;
                if (!naoProcureEstabelecimento && idUsuarioLogado > 0)
                {
                    if (EstabelecimentoID == null)
                    {
                        try
                        {
                            var usuarioEstabelecimento = await _context.UsuariosEstabelecimento.AsNoTracking().FirstOrDefaultAsync(x => x.Id == idUsuarioLogado);
                            if (usuarioEstabelecimento != null && usuarioEstabelecimento.Id_Estabelecimento!=null)
                            {
                                idEstabelecimento = (int)usuarioEstabelecimento.Id_Estabelecimento;
                                if (string.IsNullOrEmpty(NomeEstabelecimento))
                                {
                                    var estabelecimento = await _context.Estabelecimento.AsNoTracking().FirstOrDefaultAsync(x => x.Id == usuarioEstabelecimento.Id_Estabelecimento);
                                    if (estabelecimento != null)
                                        NomeEstabelecimento = estabelecimento.Nome;
                                }

                            }
                        }
                        catch (Exception erroEstab)
                        {
                            await LoggerErro(session, _context, "HelperControllers", MethodBase.GetCurrentMethod().Name, erroEstab, naoProcureEstabelecimento: true);
                        }
                    }
                }
                LogsViewModel log = new LogsViewModel();
                log.Controller = controller;
                log.Metodo = metodo;
                log.Usuario = idUsuarioLogado.ToString();
                log.Estabelecimento = NomeEstabelecimento;
                log.EstabelecimentoID = idEstabelecimento;
                log.Erro = erro.ToString();
                log.Data = DateTime.Now;

                _context.Add(log);
                await _context.SaveChangesAsync();

            }
            catch (Exception err)
            {
                //Como deu erro ao salvar um log de erro vou só mostrar no console
                Console.WriteLine(err.ToString());
            }
        }

        public static async Task LoggerErro(int idUsuario, GuidemeDbContext _context, string controller, string metodo, Exception erro, string Estabelecimento = null,
           int? EstabelecimentoID = null, bool naoProcureEstabelecimento = false)
        {
            try
            {
                int idEstabelecimento = 0;
                string NomeEstabelecimento = Estabelecimento;
                if (!naoProcureEstabelecimento && idUsuario > 0)
                {
                    if (EstabelecimentoID == null)
                    {
                        try
                        {
                            var usuarioEstabelecimento = await _context.UsuariosEstabelecimento.AsNoTracking().FirstOrDefaultAsync(x => x.Id == idUsuario);
                            if (usuarioEstabelecimento != null && usuarioEstabelecimento.Id_Estabelecimento!=null)
                            {
                                idEstabelecimento = (int)usuarioEstabelecimento.Id_Estabelecimento;
                                if (string.IsNullOrEmpty(NomeEstabelecimento))
                                {
                                    var estabelecimento = await _context.Estabelecimento.AsNoTracking().FirstOrDefaultAsync(x => x.Id == usuarioEstabelecimento.Id_Estabelecimento);
                                    if (estabelecimento != null)
                                        NomeEstabelecimento = estabelecimento.Nome;
                                }

                            }
                        }
                        catch (Exception erroEstab)
                        {
                            await LoggerErro(idUsuario, _context, "HelperControllers", MethodBase.GetCurrentMethod().Name, erroEstab, naoProcureEstabelecimento: true);
                        }
                    }
                }
                LogsViewModel log = new LogsViewModel();
                log.Controller = controller;
                log.Metodo = metodo;
                log.Usuario = idUsuario.ToString();
                log.Estabelecimento = NomeEstabelecimento;
                log.EstabelecimentoID = idEstabelecimento;
                log.Erro = erro.ToString();
                log.Data = DateTime.Now;

                _context.Add(log);
                await _context.SaveChangesAsync();

            }
            catch (Exception err)
            {
                //Como deu erro ao salvar um log de erro vou só mostrar no console
                Console.WriteLine(err.ToString());
            }
        }
        private static async Task<List<SelectListItem>> MontaSelectItemTags(int idUsuario, GuidemeDbContext _context, EnumTipoTag? tipoTag = EnumTipoTag.NaoCadastrada)
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            var usuario = await _context.UsuariosEstabelecimento.AsNoTracking().FirstOrDefaultAsync(x => x.Id == idUsuario);
            if (usuario != null)
            {
                List<TagViewModel> tagsDisponiveis = new List<TagViewModel>();
                if (tipoTag != null)
                {
                    tagsDisponiveis = await _context.Tags.AsNoTracking().Where(x => x.EstabelecimentoId == usuario.Id_Estabelecimento &&
                    x.tipoTag == (int)tipoTag).ToListAsync();
                }
                else
                    tagsDisponiveis = await _context.Tags.AsNoTracking().ToListAsync();



                foreach (TagViewModel tag in tagsDisponiveis)
                    lista.Add(new SelectListItem(tag.Nome, tag.Id.ToString()));

                
            }
            return lista;
        }

        public static async Task<List<SelectListItem>> GetListaTags(ISession session, GuidemeDbContext _context,EnumTipoTag? tipoTag=EnumTipoTag.NaoCadastrada)
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            try
            {
                int idUsuario = HelperControllers.GetUserLogadoID(session);

                lista = await  MontaSelectItemTags(idUsuario, _context, tipoTag);
            }
            catch (Exception err)
            {

            }
            return lista;
        }

        public static async Task<List<SelectListItem>> GetListaTags(int idUsuario,GuidemeDbContext _context, EnumTipoTag? tipoTag = EnumTipoTag.NaoCadastrada)
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            try
            {
                lista = await MontaSelectItemTags(idUsuario, _context, tipoTag);
            }
            catch (Exception err)
            {

            }
            return lista;
        }
        public static List<SelectListItem> GetListaDirecoes()
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            try
            {
                var direcoes =System.Enum.GetNames(typeof(EnumDirecao)).ToList();
                for (int n = 0; n < direcoes.Count; n++)
                    lista.Add(new SelectListItem(direcoes[n], n.ToString()));
               
            }
            catch (Exception err)
            {

            }
            return lista;
        }

        public static int GetUserLogadoID(ISession session)
        {
            int? logado = session.GetInt32("UserId");
            if (logado != null && logado > 0)
                return (int)logado;
            else
                return -1;
        }
    }
}
