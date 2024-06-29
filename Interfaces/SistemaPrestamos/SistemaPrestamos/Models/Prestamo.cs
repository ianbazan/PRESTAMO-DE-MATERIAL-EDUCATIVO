using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPrestamos.Models
{
    public class Prestamo
    {
        public int IdPrestamo { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public int Solicitud_IdSolicitud { get; set; }
        public string? Estado { get; set; }
        public DateTime? Fecha_Dev_Real { get; set; }


        // Campos adicionales
        [NotMapped]
        public int Material_CodMaterial { get; set; }
        [NotMapped]
        public DateTime FechaDevolucion { get; set; }
        [NotMapped]
        public int MaterialesEscanear { get; set; }
        [NotMapped]
        public int MaterialesEscaneados { get; set; }

        public Solicitud? Solicitud { get; set; }
    }
}
