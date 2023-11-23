using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuideMeServerMVC.Models
{
    [Table("tbLogs")]
    public class LogsViewModel
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }
        [Column("Controller")]
        public string Controller { get; set; }
        [Column("Metodo")]
        public string Metodo { get; set; }
        [Column("UsuarioLogado")]
        public string Usuario { get; set; }
        [Column("Estabelecimento")]
        public string Estabelecimento { get; set; }
        [Column("EstabelecimentoID")]
        public int? EstabelecimentoID { get; set; }
        [Column("Erro")]
        public string Erro { get; set; }
        [Column("Data")]
        public DateTime Data { get; set; }
    }
}
