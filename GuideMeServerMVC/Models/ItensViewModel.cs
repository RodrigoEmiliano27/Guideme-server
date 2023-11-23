
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuideMeServerMVC.Models
{
    [Table("tbItens")]
    public class ItensViewModel:BaseViewModel
    {
        
        [Column("ID_TAG")]
        public int TAG_id { get; set; }
        [Column("NOME")]
        public string Nome { get; set; }
        [Column("DESCRICAO")]
        public string Descricao { get; set; }

        [NotMapped]
        public List<SelectListItem> TagsDiponiveis { get; set; } = new List<SelectListItem>();

        [NotMapped]
        public bool Navegavel { get; set; } = false;

    }
}
