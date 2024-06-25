using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPrestamos.Models;
using SistemaPrestamos.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaPrestamos.Controllers
{
    public class PrestamosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrestamosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var viewModel = new PrestamoViewModel
            {
                Solicitudes = _context.Solicitud.Where(s => s.Estado == "Generado").ToList(),
                Prestamo = new Prestamo()
            };
            if (viewModel.Prestamo == null)
            {
                // Esto no debería suceder, pero por si acaso
                throw new Exception("El objeto Prestamo no se ha inicializado correctamente.");
            }
            return View(viewModel);
        }

        // Acción para mostrar solicitudes con estado "Generado"
        public IActionResult BuscarSolicitudPrestamo()
        {
            var solicitudes = _context.Solicitud.Where(s => s.Estado == "Generado").ToList();
            return PartialView("_SolicitudesGeneradas", solicitudes);
        }

        // Acción para mostrar el formulario de registro de préstamo
        public IActionResult MostrarFormularioPrestamo(int id)
        {
            var solicitud = _context.Solicitud
                .Include(s => s.Material)
                .Include(s => s.Alumno)
                .FirstOrDefault(s => s.IdSolicitud == id);

            if (solicitud == null)
            {
                return NotFound();
            }

            var prestamo = new Prestamo
            {
                Solicitud_IdSolicitud = solicitud.IdSolicitud,
                Material_CodMaterial = solicitud.Material_CodMaterial,
                FechaPrestamo = DateTime.Now,
                Estado = "Activo"
            };

            return PartialView("_RegistrarPrestamoModal", prestamo);
        }

        // Acción para registrar préstamo
        [HttpPost]
        public async Task<IActionResult> RegistrarPrestamo(Prestamo prestamo, int materialesEscaneados)
        {
            if (ModelState.IsValid)
            {
                // Establece el estado y materiales escaneados
                prestamo.Estado = "Activo";
                prestamo.MaterialesEscaneados = materialesEscaneados;

                // Calcula la fecha de devolución y otros campos necesarios
                prestamo.FechaDevolucion = prestamo.FechaPrestamo.AddDays(7);

                // Agrega el préstamo a la base de datos
                _context.Prestamo.Add(prestamo);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return PartialView("_RegistrarPrestamoModal", prestamo);
        }

        // Acción para actualizar préstamo - GET
        public IActionResult ActualizarPrestamo(int id)
        {
            var prestamo = _context.Prestamo.Find(id);
            if (prestamo == null)
            {
                return NotFound();
            }
            return PartialView("_ActualizarPrestamo", prestamo);
        }

        // Acción para actualizar préstamo - POST
        [HttpPost]
        public async Task<IActionResult> ActualizarPrestamo(Prestamo prestamo)
        {
            if (ModelState.IsValid)
            {
                _context.Prestamo.Update(prestamo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return PartialView("_ActualizarPrestamo", prestamo);
        }

        // Acción para registrar devolución - GET
        public IActionResult RegistrarDevolucion(int id)
        {
            var prestamo = _context.Prestamo.Find(id);
            if (prestamo == null)
            {
                return NotFound();
            }
            return PartialView("_RegistrarDevolucion", prestamo);
        }

        // Acción para registrar devolución - POST
        [HttpPost]
        public async Task<IActionResult> RegistrarDevolucion(int id, DateTime fechaDevolucion)
        {
            var prestamo = await _context.Prestamo.FindAsync(id);
            if (prestamo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                prestamo.FechaDevolucion = fechaDevolucion;
                _context.Prestamo.Update(prestamo);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return PartialView("_RegistrarDevolucion", prestamo);
        }
    }
}