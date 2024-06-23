namespace SistemaPrestamos.Models
{
    public class PrestamoViewModel
    {
        public IEnumerable<SistemaPrestamos.Models.Solicitud> Solicitudes { get; set; }
        public SistemaPrestamos.Models.Prestamo Prestamo { get; set; }
    }
}
