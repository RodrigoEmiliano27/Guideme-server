using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuideMeServerMVC.Models
{
    [Table("tbAppLogin")]
    public class AppLoginViewModel
    {
        [Key]
        public int Id { get; set; } 
        public string Login { get; set; }
        public string Senha { get; set; }
    }
}
