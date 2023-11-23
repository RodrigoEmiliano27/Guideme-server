using GuideMeServerMVC.Models;

namespace GuideMeServerMVC.TO
{
    public class ContainerAssociacaoTagsTO
    {
        public TagViewModel TagOriginal { get; set; }
        public List<AssociacaoTagTO> Relacionamentos { get; set; } = new List<AssociacaoTagTO>();
    }
}
