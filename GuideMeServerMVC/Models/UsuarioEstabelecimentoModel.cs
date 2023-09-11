using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuideMeServerMVC.Models
{
    [Table("tbUsuariosEstabelecimento")]
    public class UsuarioEstabelecimentoModel
    {
        [Key]
        public int Id { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        [Column("ADMINISTRADOR")]
        public bool admin { get; set; }
    }
}
