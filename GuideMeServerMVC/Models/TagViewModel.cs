using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuideMeServerMVC.Models
{
    [Table("tbTags")]
    public class TagViewModel
    {
        [Key]
        public int Id { get; set; }
        [Column("TAG")]
        public string TagId { get; set; }
        [Column("ID_ESTABELECIMENTO")]
        public int EstabelecimentoId { get; set; }
        [Column("[ID_TIPO_TAG]")]
        public int tipoTag { get; set; }
    }
}
