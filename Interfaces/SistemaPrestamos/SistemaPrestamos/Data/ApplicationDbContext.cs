using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaPrestamos.Models;

namespace SistemaPrestamos.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Agregar Modelos
        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Encargado> Encargados { get; set; }
        public DbSet<Material> Materiales { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }
        public DbSet<Penalizacion> Penalizaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Alumno)
                .WithMany(a => a.Prestamos)
                .HasForeignKey(p => p.AlumnoId);

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Material)
                .WithMany(m => m.Prestamos)
                .HasForeignKey(p => p.MaterialId);

            modelBuilder.Entity<Penalizacion>()
                .HasOne(p => p.Prestamo)
                .WithMany(pr => pr.Penalizaciones)
                .HasForeignKey(p => p.PrestamoId);
        }
    }
}
