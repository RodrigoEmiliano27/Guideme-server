using GuideMeServerMVC.Data;
using GuideMeServerMVC.Enum;
using GuideMeServerMVC.Models;
using GuideMeServerMVC.Utils;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GuideMeServerMVC.Services
{
    public class ItensService : ServiceBase<ItensViewModel>
    {
        public ItensService(GuidemeDbContext context) : base(context)
        {
        }

        public async Task<bool> isTagItem(TagViewModel tag)
        {
            return await _context.Itens.AsNoTracking().AnyAsync(x => x.TAG_id == tag.Id);
        }

        public override async Task<bool> SaveAsync(ItensViewModel model)
        {
            TagService _service = new TagService(_context);

            var tagSelecionada = await _service.GetById(model.TAG_id);
            if (tagSelecionada != null)
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    TagViewModel newTag = tagSelecionada;
                    newTag.tipoTag = (int)EnumTipoTag.itens;

                    await _service.UpdateAsync(newTag);

                    _context.Add(model);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    return true;

                }
                catch (Exception err)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
                finally
                {
                    transaction.Dispose();
                }
            }

            return false;
        }
        public override async Task<bool> UpdateAsync(ItensViewModel model)
        {
            TagService _service = new TagService(_context);
            var itenOld = await GetById(model.Id);

            if (model != null && itenOld!=null)
            {
                var tagSelecionada = await _service.GetById(model.TAG_id);
              
                using var transaction = _context.Database.BeginTransaction();
                var tagAntiga = await _service.GetById(itenOld.TAG_id);
                if (tagAntiga != null)
                {

                    try
                    {
                        if (tagAntiga.Id != tagSelecionada.Id)
                        {
                            TagViewModel newTag = tagAntiga;
                            newTag.tipoTag = (int)EnumTipoTag.NaoCadastrada;

                             await _service.UpdateAsync(newTag);

                            newTag = tagSelecionada;
                            newTag.tipoTag = (int)EnumTipoTag.itens;

                            _context.Update(newTag);
                            await _context.SaveChangesAsync();
                        }
                        _context.Update(model);
                        await _context.SaveChangesAsync();

                        transaction.Commit();

                    }
                    catch (Exception err)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                    finally
                    {
                        transaction.Dispose();
                    }

                }
            }

            return false;
        }

        public async Task<ItensViewModel> GetItemByTag(TagViewModel tag)
        {
            ItensViewModel model = null;

            model = await _context.Itens.AsNoTracking().FirstOrDefaultAsync(x => x.TAG_id == tag.Id);

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
                        await _context.Itens.Where(x => x.Id == id).ExecuteDeleteAsync();

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

        public override Dictionary<string, string> ValidarDados(ItensViewModel model)
        {
            Dictionary<string, string> retorno = new Dictionary<string, string>();

            if (model.TAG_id <= 0)
                retorno.Add("TagSelecionada", "Por favor selecione uma tag disponível!");

            if (string.IsNullOrEmpty(model.Nome) || string.IsNullOrEmpty(model.Nome.Trim()))
                retorno.Add("Nome", "Nome inválido!");

            if (string.IsNullOrEmpty(model.Descricao) || string.IsNullOrEmpty(model.Descricao.Trim()))
                retorno.Add("Descricao", "Descricao inválida!");

            if (retorno.Count > 0)
                return retorno;
            else
                return null;
        }
    }
}
