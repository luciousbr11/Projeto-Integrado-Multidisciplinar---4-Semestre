using Microsoft.EntityFrameworkCore;
using GestaoChamadosAI_Web.Models;

namespace GestaoChamadosAI_Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Chamado> Chamados { get; set; }
        public DbSet<MensagemChamado> MensagensChamados { get; set; }
        public DbSet<AnexoMensagem> AnexosMensagens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Chamado>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Chamados)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chamado>()
                .HasOne(c => c.SuporteResponsavel)
                .WithMany()
                .HasForeignKey(c => c.SuporteResponsavelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MensagemChamado>()
                .HasOne(m => m.Chamado)
                .WithMany(c => c.Mensagens)
                .HasForeignKey(m => m.ChamadoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MensagemChamado>()
                .HasOne(m => m.Usuario)
                .WithMany()
                .HasForeignKey(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AnexoMensagem>()
                .HasOne(a => a.MensagemChamado)
                .WithMany(m => m.Anexos)
                .HasForeignKey(a => a.MensagemChamadoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
