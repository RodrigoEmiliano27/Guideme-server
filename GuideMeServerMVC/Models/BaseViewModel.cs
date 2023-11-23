using System.ComponentModel.DataAnnotations;

namespace GuideMeServerMVC.Models
{
    public abstract class BaseViewModel
    {
        [Key]
        public int Id { get; set; }
    }
}
