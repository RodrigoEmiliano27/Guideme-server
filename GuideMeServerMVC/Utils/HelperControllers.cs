using GuideMeServerMVC.Data;
using GuideMeServerMVC.Enum;
using GuideMeServerMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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

        public static async Task<List<SelectListItem>> GetListaTags(ISession session, GuidemeDbContext _context,EnumTipoTag? tipoTag=EnumTipoTag.NaoCadastrada)
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            try
            {
                int idUsuario = HelperControllers.GetUserLogadoID(session);

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
