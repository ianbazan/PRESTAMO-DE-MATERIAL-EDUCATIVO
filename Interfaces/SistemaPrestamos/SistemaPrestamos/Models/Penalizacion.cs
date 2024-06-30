namespace SistemaPrestamos.Models
{
    public class Penalizacion
    {
        public int IdPenalizacion { get; set; }
        public DateTime FechaPenalizacion { get; set; }
        public string Descripcion { get; set; }
        public int Prestamo_IdPrestamo { get; set; }

        public Prestamo Prestamo { get; set; }
    }
}
