using Microsoft.EntityFrameworkCore;
using ProyectoFinal_DuranDaniel.Models;

namespace ProyectoFinal_DuranDaniel.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Rol>          Roles          { get; set; }
        public DbSet<Usuario>      Usuarios       { get; set; }
        public DbSet<Carrera>      Carreras       { get; set; }
        public DbSet<Curso>        Cursos         { get; set; }
        public DbSet<Matricula>    Matriculas     { get; set; }
        public DbSet<MaterialCurso> MaterialesCurso { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Curso>()
                .HasOne(c => c.Carrera)
                .WithMany(ca => ca.Cursos)
                .HasForeignKey(c => c.CarreraId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Curso>()
                .HasOne(c => c.Docente)
                .WithMany(u => u.CursosQueImparte)
                .HasForeignKey(c => c.DocenteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Matricula>()
                .HasOne(m => m.Estudiante)
                .WithMany(u => u.Matriculas)
                .HasForeignKey(m => m.EstudianteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Matricula>()
                .HasOne(m => m.Curso)
                .WithMany(c => c.Matriculas)
                .HasForeignKey(m => m.CursoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MaterialCurso>()
                .HasOne(mc => mc.Curso)
                .WithMany()
                .HasForeignKey(mc => mc.CursoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MaterialCurso>()
                .HasOne(mc => mc.SubidoPor)
                .WithMany()
                .HasForeignKey(mc => mc.SubidoPorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
