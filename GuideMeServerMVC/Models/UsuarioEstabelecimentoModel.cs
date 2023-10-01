using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuideMeServerMVC.Models
{
    [Table("tbUsuariosEstabelecimento")]
    public class UsuarioEstabelecimentoModel
    {
        public UsuarioEstabelecimentoModel()
        {
            Login = string.Empty;
            Senha = string.Empty;
            Id = 0;
            Admin = false;
            Id_Estabelecimento = 0;
        }

        [Key]
        public int Id { get; set; }
        [Column("LOGIN")]
        public string Login { get; set; }
        [Column("SENHA")]
        public string Senha { get; set; }
        [Column("ADMINISTRADOR")]
        public bool Admin { get; set; }
        [Column("Id_Estabelecimento")]
        public int Id_Estabelecimento { get; set; }


    }
}
