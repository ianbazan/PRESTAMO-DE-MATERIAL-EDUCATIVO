using Microsoft.AspNetCore.Mvc;
using SistemaPrestamos.Data;
using SistemaPrestamos.Models;
using System.Linq;

namespace SistemaPrestamos.Controllers
{
    public class AlumnoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlumnoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Acción para mostrar la página de solicitud de préstamo
        public IActionResult SolicitudPrestamo()
        {
            // Aquí puedes cargar los materiales disponibles u otras opciones necesarias
            var materiales = _context.Materiales.ToList();
            return View(materiales);
        }

        // Acción para procesar la solicitud de préstamo
        [HttpPost]
        public IActionResult SolicitudPrestamo(int materialId)
        {
            // Aquí procesas la solicitud de préstamo
            // Por ejemplo, puedes crear un nuevo préstamo en la base de datos
            // y actualizar la cantidad disponible del material
            // (Implementación según tu lógica de negocio)
            // ...

            return RedirectToAction("Index", "Home");
        }
    }
}
