using GuideMeServerMVC.Enum;
using GuideMeServerMVC.Models;

namespace GuideMeServerMVC.TO
{
    public class AssociacaoTagTO
    {
        
        public TagViewModel TagPrincipal { get; set; }
        public TagViewModel TagSecundaria { get; set; }
        public EnumDirecao Direcao { get; set; }

    }
}
