using System.ComponentModel.DataAnnotations;

namespace SistemaPrestamos.Models
{
    public class Penalizacion
    {
        public int Id { get; set; }
        [Required]
        public int PrestamoId { get; set; }
        public Prestamo Prestamo { get; set; }

        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
    }
}
