using GuideMeServerMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace GuideMeServerMVC.Data
{
    public class GuidemeDbContext:DbContext
    {
        public GuidemeDbContext() 
        {
            
        }
        public DbSet<UsuarioViewModel> Usuarios { get; set; } = null!;


    }
}
