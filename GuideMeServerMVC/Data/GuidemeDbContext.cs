using GuideMeServerMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace GuideMeServerMVC.Data
{
    public class GuidemeDbContext:DbContext
    {
        
        public GuidemeDbContext(DbContextOptions<GuidemeDbContext> options)
      : base(options)
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // You don't actually ever need to call this
        }
        public GuidemeDbContext()
        {
           
        }

        public DbSet<UsuarioViewModel> Usuarios { get; set; } = null!;
        public DbSet<AppLoginViewModel> AppLogin { get; set; } = null!;



    }
}
