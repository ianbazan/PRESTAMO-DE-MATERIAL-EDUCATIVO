using System.ComponentModel.DataAnnotations;

namespace SistemaPrestamos.Models
{
    public class Alumno
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }

        public ICollection<Prestamo> Prestamos { get; set; }
    }
}
