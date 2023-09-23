
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuideMeServerMVC.Models
{
    [Table("tbLugares")]
    public class LugaresViewModel
    {
        [Key]
        public int Id { get; set; }
        [Column("ID_TAG")]
        public int TAG_id { get; set; }
        [Column("NOME")]
        public string Nome { get; set; }
        [Column("DESCRICAO")]
        public string Descricao { get; set; }
    }
}
