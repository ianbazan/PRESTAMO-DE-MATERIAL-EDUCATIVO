using System.ComponentModel.DataAnnotations;

namespace SistemaPrestamos.Models
{
    public class Material
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int CantidadDisponible { get; set; }
        
        // Relación con Prestamo
        public ICollection<Prestamo> Prestamos { get; set; }
    }
}
