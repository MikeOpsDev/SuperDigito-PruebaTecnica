using Microsoft.EntityFrameworkCore;

namespace SuperDigitoApp.Models
{
    public class SuperDigitoDbContext : DbContext
    {
        public SuperDigitoDbContext(DbContextOptions<SuperDigitoDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<HistorialCalculo> HistorialCalculos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Username).IsUnique();
            });

            modelBuilder.Entity<HistorialCalculo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(d => d.Usuario)
                      .WithMany(p => p.Historiales)
                      .HasForeignKey(d => d.UsuarioId);
            });
        }
    }
}
