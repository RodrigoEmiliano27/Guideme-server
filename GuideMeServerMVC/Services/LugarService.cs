using GuideMeServerMVC.Data;
using GuideMeServerMVC.Enum;
using GuideMeServerMVC.Models;
using GuideMeServerMVC.Utils;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GuideMeServerMVC.Services
{
    public class LugarService : ServiceBase<LugaresViewModel>
    {
        public LugarService(GuidemeDbContext context) : base(context)
        {
        }
        public async Task<LugaresViewModel> GetLugarByTag(TagViewModel tag)
        {
            LugaresViewModel model = null;

            model = await _context.Lugares.AsNoTracking().FirstOrDefaultAsync(x => x.TAG_id == tag.Id);

            return model;
        }

        public override async Task<bool> SaveAsync(LugaresViewModel model)
        {
            TagService _service = new TagService(_context);
            var tagSelecionada = await _service.GetById(model.TAG_id);

            if (tagSelecionada != null)
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    TagViewModel newTag = tagSelecionada;
                    newTag.tipoTag = (int)EnumTipoTag.lugar;

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
        public override async Task<bool> UpdateAsync(LugaresViewModel model)
        {
            TagService _service = new TagService(_context);
            var lugarOld = await GetById(model.Id);

            if (model != null && lugarOld != null)
            {
                var tagSelecionada = await _service.GetById(model.TAG_id);

                using var transaction = _context.Database.BeginTransaction();
                var tagAntiga = await _service.GetById(lugarOld.TAG_id);
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
                            newTag.tipoTag = (int)EnumTipoTag.lugar;

                            await _service.UpdateAsync(newTag);
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


        public async Task<bool> Delete(int id, int idUsuario, bool deletarTag = false)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var lugar = await _context.Lugares.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (lugar != null)
                {
                    var tagCadastrada = await _context.Tags.AsNoTracking().FirstOrDefaultAsync(x => x.Id == lugar.TAG_id);
                    if (tagCadastrada != null)
                    {
                        await _context.Lugares.Where(x => x.Id == id).ExecuteDeleteAsync();

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
        public async Task<bool> isTagLugar(TagViewModel tag)
        {
            return await _context.Lugares.AsNoTracking().AnyAsync(x=>x.TAG_id==tag.Id);
        }

        public override Dictionary<string, string> ValidarDados(LugaresViewModel model)
        {
            Dictionary<string, string> erros = new Dictionary<string, string>();
            if (model.TAG_id <= 0)
                erros.Add("TagSelecionada", "Por favor selecione uma tag disponível!");

            if (string.IsNullOrEmpty(model.Nome) || string.IsNullOrEmpty(model.Nome.Trim()))
                erros.Add("Nome", "Nome inválido!");

            if (string.IsNullOrEmpty(model.Descricao) || string.IsNullOrEmpty(model.Descricao.Trim()))
                erros.Add("Descricao", "Descricao inválida!");

            if (erros.Count > 0)
                return erros;
            else
                return null;
        }
    }
}
