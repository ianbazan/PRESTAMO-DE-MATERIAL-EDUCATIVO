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
        public int Material_CodMaterial { get; set; }
        public DateTime FechaDevolucion { get; set; }
        public int MaterialesEscanear { get; set; }
        public int MaterialesEscaneados { get; set; }

        public Solicitud? Solicitud { get; set; }
    }
}
