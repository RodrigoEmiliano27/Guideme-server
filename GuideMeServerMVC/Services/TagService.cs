using GuideMeServerMVC.Data;
using GuideMeServerMVC.Enum;
using GuideMeServerMVC.Models;
using GuideMeServerMVC.TO;
using GuideMeServerMVC.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GuideMeServerMVC.Services
{
    public class TagService: ServiceBase<TagViewModel>
    {
        public TagService(GuidemeDbContext context):base(context)
        {
           
        }

        public override Dictionary<string, string> ValidarDados(TagViewModel model)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string,string> ValidarDadosAssociacaoTags(CadastrarAssociacaoTagsTO model, out TagViewModel _menorTag, out TagViewModel _maiorTag)
        {
            Dictionary<string, string> erros = new Dictionary<string, string>();
            int menorTag = 0;
            int maiorTag = 0;
            string menorTagNome = "";
            string maiorTagNome = "";
            _menorTag = null;
            _maiorTag = null;

            if (model.TagPrincipalSelecionada <= 0)
                erros.Add("TagPrincipalSelecionada", "Por favor selecione uma tag disponível!");

            if (model.TagSecundariaSelecionada <= 0)
                erros.Add("TagSecundariaSelecionada", "Por favor selecione uma tag disponível!");

            if (model.TagSecundariaSelecionada == model.TagPrincipalSelecionada)
            {
                erros.Add("TagPrincipalSelecionada", "A tag principal e a secundaria não podem ser as mesmas!");
                erros.Add("TagSecundariaSelecionada", "A tag principal e a secundaria não podem ser as mesmas!");
            }


            if (model.TagPrincipalSelecionada < model.TagSecundariaSelecionada)
            {
                menorTag = model.TagPrincipalSelecionada;
                menorTagNome = "TagPrincipalSelecionada";
                maiorTag = model.TagSecundariaSelecionada;
                maiorTagNome = "TagSecundariaSelecionada";

            }
            else
            {
                menorTag = model.TagSecundariaSelecionada;
                menorTagNome = "TagSecundariaSelecionada";
                maiorTag = model.TagPrincipalSelecionada;
                maiorTagNome = "TagPrincipalSelecionada";
            }


            if (model.TagPrincipalSelecionada > 0 && model.TagSecundariaSelecionada > 0 &&
                model.TagSecundariaSelecionada != model.TagPrincipalSelecionada)
            {
                _menorTag = _context.Tags.AsNoTracking().FirstAsync(x => x.Id == menorTag).Result;
                _maiorTag = _context.Tags.AsNoTracking().FirstAsync(x => x.Id == maiorTag).Result;

                if (_menorTag == null)
                    erros.Add(menorTagNome, "Tag não foi encontrada!");
                if (_maiorTag == null)
                    erros.Add(maiorTagNome, "Tag não foi encontrada!");



            }

            if (erros.Count > 0)
                return erros;
            else
                return null;

        }
        public async Task<CadastrarAssociacaoTagsTO> GetDadosCadastroAssociacao(int idUsuario)
        {
            var tagsNaoCadastradas = await HelperControllers.GetListaTags(idUsuario, _context, EnumTipoTag.NaoCadastrada);
            var tagsLugares = await HelperControllers.GetListaTags(idUsuario, _context, EnumTipoTag.lugar);
            var tagsItens = await HelperControllers.GetListaTags(idUsuario, _context, EnumTipoTag.itens);
            List<SelectListItem> tagsDisponiveis = new List<SelectListItem>();

            foreach (var tag in tagsNaoCadastradas)
                tagsDisponiveis.Add(tag);

            foreach (var tag in tagsLugares)
                tagsDisponiveis.Add(tag);

            foreach (var tag in tagsItens)
                tagsDisponiveis.Add(tag);

            var direcoesDisponiveis = System.Enum.GetNames(typeof(EnumDirecao)).ToList();
            CadastrarAssociacaoTagsTO associacaoTagsTO = new CadastrarAssociacaoTagsTO();
            associacaoTagsTO.DirecoesDisponiveis = HelperControllers.GetListaDirecoes();
            associacaoTagsTO.TagsDiponiveis = tagsDisponiveis;
            return associacaoTagsTO;
        }

        public async Task<List<TagViewModel>> GetTagsUsuario(int idUsuario)
        {
            List<TagViewModel> lista = new List<TagViewModel>();
            var user = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Id == idUsuario);
            if (user != null)
            {
                lista = _context.Tags.Where(o => o.EstabelecimentoId == user.Id_Estabelecimento).ToList();
                return lista;
            }
            else
                return null;
            
        }
      
        public async Task<ContainerAssociacaoTagsTO> GetAssociacoesTag(int idTag)
        {
            ContainerAssociacaoTagsTO associacoes = new ContainerAssociacaoTagsTO();
            TagViewModel model = await _context.Tags.AsNoTracking().FirstOrDefaultAsync(x => x.Id == idTag);
            if (model != null)
            {
                List<AssociacaoTagTO> lista = new List<AssociacaoTagTO>();
                var tagsRelacionamento = _context.TagsPai.AsNoTracking().Where(x => x.Id_Tag_Pai == model.Id || x.Id_Tag == model.Id).ToList();
                foreach (var relacionamento in tagsRelacionamento)
                {
                    var TagPrincipal = await _context.Tags.AsNoTracking().FirstAsync(x => x.Id == relacionamento.Id_Tag_Pai);
                    var TagSecundaria = await _context.Tags.AsNoTracking().FirstAsync(x => x.Id == relacionamento.Id_Tag);

                    if (TagPrincipal != null && TagSecundaria != null)
                        lista.Add(new AssociacaoTagTO() { TagPrincipal = TagPrincipal, TagSecundaria = TagSecundaria, Direcao = (EnumDirecao)relacionamento.Direcao });
                }

                associacoes.TagOriginal = model;
                associacoes.Relacionamentos = lista;

                return associacoes;

            }
            else
                return null;

        }

        public async Task SalvarAssociacao(CadastrarAssociacaoTagsTO model,TagViewModel menorTag, TagViewModel maiorTag)
        {
            var TagRelacionamento = await _context.TagsPai.AsNoTracking().FirstOrDefaultAsync(x => x.Id_Tag_Pai == menorTag.Id
                     && x.Id_Tag == maiorTag.Id);

            TagsPaiViewModel tagPai = new TagsPaiViewModel();
            tagPai.Id_Tag_Pai = menorTag.Id;
            tagPai.Id_Tag = maiorTag.Id;
            tagPai.Direcao = model.DirecaoSelecionada;
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                if (TagRelacionamento == null)
                {
                    _context.Add(tagPai);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    tagPai.Id = TagRelacionamento.Id;
                    _context.Update(tagPai);
                    await _context.SaveChangesAsync();
                }
                TagViewModel tagAux = new TagViewModel();
                if (menorTag.tipoTag == (int)EnumTipoTag.NaoCadastrada)
                {
                    tagAux = menorTag;
                    tagAux.tipoTag =(int)EnumTipoTag.localizacao;
                    _context.Update(tagAux);

                }

                if (maiorTag.tipoTag == (int)EnumTipoTag.NaoCadastrada)
                {
                    tagAux = maiorTag;
                    tagAux.tipoTag = (int)EnumTipoTag.localizacao;
                    _context.Update(tagAux);

                }

                await transaction.CommitAsync();

            }
            catch (Exception err)
            {
                transaction.Rollback();
            }

           
        }

        public override async Task<bool> Delete(int id, int idUsuario, bool deletarTag = false)
        {
            var tag = _context.Tags.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (tag != null)
            {

                if (tag.tipoTag == (int)EnumTipoTag.lugar)
                {
                    LugarService _serviceLugar = new LugarService(_context);
                    var lugar = await _serviceLugar.GetLugarByTag(tag);
                    if (lugar != null)
                        return await _serviceLugar.Delete(lugar.Id, idUsuario, true);
                }
                else if (tag.tipoTag == (int)EnumTipoTag.itens)
                {
                    ItensService _serviceItem = new ItensService(_context);
                    var item = await _serviceItem.GetItemByTag(tag);
                    if (item != null)
                        return await _serviceItem.Delete(item.Id, idUsuario, true);

                }
                else
                {
                    _context.Remove(tag);
                    return true;
                }



            }
            else
                return false;

            return false;
        }
    }
}
