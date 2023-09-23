
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuideMeServerMVC.Models
{
    [Table("tbEstabelecimento")]
    public class EstabelecimentoViewModel
    {
        [Key]
        public int Id { get; set; }
        [Column("NOME")]
        public string Nome { get; set; }
    }
}
