using Microsoft.AspNetCore.Mvc.Rendering;

namespace GuideMeServerMVC.TO
{
    public class CadastrarAssociacaoTagsTO
    {
        public List<SelectListItem> TagsDiponiveis { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> DirecoesDisponiveis { get; set; } = new List<SelectListItem>();

        public int TagPrincipalSelecionada { get; set; }
        public int TagSecundariaSelecionada { get; set; }
        public int DirecaoSelecionada { get; set; }


    }
}
