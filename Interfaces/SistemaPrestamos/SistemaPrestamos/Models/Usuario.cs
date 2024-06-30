namespace SistemaPrestamos.Models
{
    public class Usuario
    {
        public int CodUsuario { get; set; }
        public string Contrasenia { get; set; }
        public string Role { get; set; }  // Nueva propiedad Role
    }
}
