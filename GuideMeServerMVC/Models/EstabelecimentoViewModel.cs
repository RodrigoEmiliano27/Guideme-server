namespace GuideMeServerMVC.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class EstabelecimentoViewModel
    {
        [Key]
        public int Id { get; set; }
        [Column("TAG")]
        public string TagId { get; set; }
        [Column("ID_ESTABELECIMENTO")]
        public int EstabelecimentoId { get; set; }
    }
}
