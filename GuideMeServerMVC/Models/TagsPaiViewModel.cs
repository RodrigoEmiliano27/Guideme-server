
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuideMeServerMVC.Models
{
    [Table("tbTagsPai")]
    public class TagsPaiViewModel
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("ID_TAG")]
        public int Id_Tag { get; set; }
        [Column("IDTAG_PAI")]
        public int Id_Tag_Pai { get; set; }

        [Column("DIRECAO")]
        public int Direcao { get; set; }

    }
}
