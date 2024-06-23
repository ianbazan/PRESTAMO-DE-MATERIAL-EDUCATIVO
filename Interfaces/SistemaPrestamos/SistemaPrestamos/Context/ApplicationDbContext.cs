using Microsoft.EntityFrameworkCore;
using SistemaPrestamos.Models;
using System.Collections.Generic;

namespace SistemaPrestamos.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Alumno> Alumno { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Material> Material { get; set; }
        public DbSet<Solicitud> Solicitud { get; set; }
        public DbSet<Prestamo> Prestamo { get; set; }
        public DbSet<Penalizacion> Penalizacion { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Alumno>().ToTable("Alumno");
    
            // Configuración de la clave primaria y relaciones de Usuario
            modelBuilder.Entity<Usuario>()
                .HasKey(u => u.CodUsuario);

            // Configuración de la clave primaria y relaciones de Alumno
            modelBuilder.Entity<Alumno>()
                .HasKey(a => a.Usuario_CodUsuario);

            modelBuilder.Entity<Alumno>()
                .HasOne(a => a.Usuario)
                .WithOne()
                .HasForeignKey<Alumno>(a => a.Usuario_CodUsuario);

            // Configuración de la clave primaria y relaciones de Material
            modelBuilder.Entity<Material>()
                .HasKey(m => m.CodMaterial);

            // Configuración de la clave primaria y relaciones de Solicitud
            modelBuilder.Entity<Solicitud>()
                .HasKey(s => s.IdSolicitud);

            modelBuilder.Entity<Solicitud>()
                .HasOne(s => s.Alumno)
                .WithMany()
                .HasForeignKey(s => s.Alumno_Usuario_CodUsuario);

            modelBuilder.Entity<Solicitud>()
                .HasOne(s => s.Material)
                .WithMany()
                .HasForeignKey(s => s.Material_CodMaterial);

            // Configuración de la clave primaria y relaciones de Prestamo
            modelBuilder.Entity<Prestamo>()
                .HasKey(p => p.IdPrestamo);

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Solicitud)
                .WithMany()
                .HasForeignKey(p => p.Solicitud_IdSolicitud);

            // Mapeo de propiedades adicionales en Prestamo
            modelBuilder.Entity<Prestamo>()
                .Property(p => p.Material_CodMaterial).HasColumnName("Material_CodMaterial");
            modelBuilder.Entity<Prestamo>()
                .Property(p => p.FechaDevolucion).HasColumnName("FechaDevolucion");
            modelBuilder.Entity<Prestamo>()
                .Property(p => p.MaterialesEscanear).HasColumnName("MaterialesEscanear");
            modelBuilder.Entity<Prestamo>()
                .Property(p => p.MaterialesEscaneados).HasColumnName("MaterialesEscaneados");

            // Configuración de la clave primaria y relaciones de Penalizacion
            modelBuilder.Entity<Penalizacion>()
                .HasKey(p => p.IdPenalizacion);

            modelBuilder.Entity<Penalizacion>()
                .HasOne(p => p.Prestamo)
                .WithMany()
                .HasForeignKey(p => p.Prestamo_IdPrestamo);
        }
    }
}
