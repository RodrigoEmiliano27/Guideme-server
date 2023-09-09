using GuideMeServerMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace GuideMeServerMVC.Data
{
    public class GuidemeDbContext:DbContext
    {
        
        public GuidemeDbContext(DbContextOptions options)
      : base(options)
        {
            
        }
       
        public DbSet<UsuarioViewModel> Usuarios { get; set; } = null!;
        public DbSet<AppLoginViewModel> AppLogin { get; set; } = null!;



    }
}
