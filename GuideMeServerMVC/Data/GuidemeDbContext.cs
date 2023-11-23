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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TagViewModel>().ToTable(tb => tb.HasTrigger("trigger"));
        }


        public DbSet<AppLoginViewModel> AppLogin { get; set; } = null!;
        public DbSet<EstabelecimentoViewModel> Estabelecimento { get; set; } = null!;
        public DbSet<LugaresViewModel> Lugares { get; set; } = null!;
        public DbSet<ItensViewModel> Itens { get; set; } = null!;
        public DbSet<TagViewModel> Tags { get; set; } = null!;
        public DbSet<TagsPaiViewModel> TagsPai { get; set; } = null!;
        public DbSet<UsuarioEstabelecimentoModel> UsuariosEstabelecimento { get; set; } = null!;
        public DbSet<LogsViewModel> Logs { get; set; } = null!;


    }
}
