namespace SistemaPrestamos.Models
{
    public class Solicitud
    {
        public int IdSolicitud { get; set; }
        public int Cantidad { get; set; }
        public string Estado { get; set; }
        public int Material_CodMaterial { get; set; }
        public int Alumno_Usuario_CodUsuario { get; set; }

        public Material Material { get; set; }
        public Alumno Alumno { get; set; }
    }
}
