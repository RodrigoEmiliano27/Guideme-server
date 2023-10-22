using GuideMeServerMVC.Data;
using GuideMeServerMVC.Enum;
using GuideMeServerMVC.Models;
using GuideMeServerMVC.Utils;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GuideMeServerMVC.Services
{
    public class TagsPaiService : ServiceBase<TagsPaiViewModel>
    {
        public TagsPaiService(GuidemeDbContext context) : base(context)
        {
        }

        public async Task<bool> DeletarRelacionamentos(int idTagPrincipal, int idTagSecundaria)
        {
            var relacionamento = await _context.TagsPai.AsNoTracking().FirstOrDefaultAsync(x => x.Id_Tag_Pai == idTagPrincipal && x.Id_Tag == idTagSecundaria);
            if (relacionamento != null)
            {
                _context.Remove(relacionamento);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<List<TagsPaiViewModel>> GetRelacionamentosTag(TagViewModel tag)
        {
            List<TagsPaiViewModel> model = null;

            model =  await _context.TagsPai.AsNoTracking().Where(x => x.Id_Tag_Pai== tag.Id || x.Id_Tag==tag.Id).ToListAsync();

            return model;
        }

        public async Task<bool> Delete(int id, int idUsuario, bool deletarTag = false)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var lugar = await _context.Itens.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (lugar != null)
                {
                    var tagCadastrada = await _context.Tags.AsNoTracking().FirstOrDefaultAsync(x => x.Id == lugar.TAG_id);
                    if (tagCadastrada != null)
                    {
                        await _context.TagsPai.Where(x => x.Id_Tag_Pai == tagCadastrada.Id || x.Id_Tag == tagCadastrada.Id).ExecuteDeleteAsync();
                        if (!deletarTag)
                        {
                            TagViewModel newTag = tagCadastrada;
                            newTag.tipoTag = (int)EnumTipoTag.NaoCadastrada;

                            _context.Update(newTag);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            _context.Remove(tagCadastrada);
                            await _context.SaveChangesAsync();
                        }


                        await transaction.CommitAsync();

                    }
                    return true;
                }
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(idUsuario, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
            }
            finally
            {
                await transaction.DisposeAsync();
            }

            return false;
        }
        public override Dictionary<string, string> ValidarDados(TagsPaiViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
