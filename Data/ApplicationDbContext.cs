using BackEndHorario.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BackEndHorario.Data {
    public class ApplicationDbContext : IdentityDbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {
        }

        public DbSet<Cursos> Cursos { get; set; }
        public DbSet<Turmas> Turmas { get; set; }
        public DbSet<Docentes> Docentes { get; set; }
        public DbSet<Unidades_Curriculares> UnidadesCurriculares { get; set; }
        public DbSet<Salas> Salas { get; set; }
        public DbSet<Escolas> Escolas { get; set; }
        public DbSet<Horarios> Horarios { get; set; }
        public DbSet<Blocos> Blocos { get; set; }
        public DbSet<Utilizadores> Utilizadores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Blocos>()
                .HasOne(b => b.UnidadeCurricular)
                .WithMany(uc => uc.Blocos)
                .HasForeignKey(b => b.UnidadeCurricularId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Blocos>()
                .HasOne(b => b.Turma)
                .WithMany(t => t.Blocos)
                .HasForeignKey(b => b.TurmaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Blocos>()
                .HasOne(b => b.Docente)
                .WithMany(d => d.Blocos)
                .HasForeignKey(b => b.DocenteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Blocos>()
                .HasOne(b => b.Sala)
                .WithMany(s => s.Blocos)
                .HasForeignKey(b => b.SalaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Blocos>()
                .HasOne(b => b.Horario)
                .WithMany(h => h.Blocos)
                .HasForeignKey(b => b.HorarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Blocos>()
                .HasOne(b => b.Utilizador)
                .WithMany(u => u.Blocos)
                .HasForeignKey(b => b.UtilizadorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do comprimento máximo da coluna TipoAula
            modelBuilder.Entity<Blocos>()
                .Property(b => b.TipoAula)
                .HasMaxLength(50)  // Define 50 caracteres como tamanho máximo
                .IsRequired();     // Mantém obrigatória
        }
    }
}
