using GuideMeServerMVC.Models;

namespace GuideMeServerMVC.TO
{
    public class EstabelecimentoTagsTO
    {
        public List<TagViewModel> Tags { get; set; }
        public List<LugaresViewModel> Lugares { get; set; }

        public List<ItensViewModel> Itens { get; set; }

        public string NomeEstabelecimento { get; set; }
    }
}
