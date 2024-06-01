using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPrestamos.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaPrestamos.Controllers
{
    public class EncargadoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EncargadoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Acción para mostrar el listado de préstamos
        public async Task<IActionResult> ListadoPrestamos()
        {
            var prestamos = await _context.Prestamos
                .Include(p => p.Alumno)
                .Include(p => p.Material)
                .ToListAsync();

            return View(prestamos);
        }

        // Acción para registrar la devolución de un material
        public IActionResult RegistrarDevolucion(int prestamoId)
        {
            // Aquí puedes implementar la lógica para registrar la devolución
            return RedirectToAction(nameof(ListadoPrestamos));
        }

        // Acción para registrar una penalización
        public IActionResult RegistrarPenalizacion(int prestamoId)
        {
            // Aquí puedes implementar la lógica para registrar la penalización
            return RedirectToAction(nameof(ListadoPrestamos));
        }
    }
}
