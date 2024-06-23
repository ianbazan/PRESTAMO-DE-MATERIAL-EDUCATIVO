namespace SistemaPrestamos.Models
{
    public class Alumno
    {
        public int Usuario_CodUsuario { get; set; }
        public string? NombresApellidos { get; set; }
        public string? Estado { get; set; }
        public int DiasInhabilitado { get; set; }

        public Usuario? Usuario { get; set; }
    }
}
