
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuideMeServerMVC.Models
{
    [Table("tbTagsPai")]
    [Keyless]
    public class TagsPaiViewModel
    {
        [Column("ID_TAG")]
        public int Id_Tag { get; set; }
        [Column("IDTAG_PAI")]
        public int Id_Tag_Pai { get; set; }
    }
}
