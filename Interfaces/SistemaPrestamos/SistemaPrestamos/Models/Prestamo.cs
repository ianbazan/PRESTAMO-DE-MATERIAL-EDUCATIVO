using System.ComponentModel.DataAnnotations;

namespace SistemaPrestamos.Models
{
    public class Prestamo
    {
        public int Id { get; set; }
        [Required]
        public int AlumnoId { get; set; }
        public Alumno Alumno { get; set; }

        [Required]
        public int MaterialId { get; set; }
        public Material Material { get; set; }

        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaDevolucion { get; set; }

        public ICollection<Penalizacion> Penalizaciones { get; set; }
    }
}
