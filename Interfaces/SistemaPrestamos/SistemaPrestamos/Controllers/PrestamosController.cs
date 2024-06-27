using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPrestamos.Models;
using SistemaPrestamos.Context;
using System;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnector;

namespace SistemaPrestamos.Controllers
{
    public class PrestamosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrestamosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Acción para mostrar la vista de registrar préstamo
        public IActionResult RegistrarPrestamo()
        {
            var solicitudes = _context.Solicitud.Where(s => s.Estado == "Generado").ToList();
            return View(solicitudes);
        }

        // Acción para mostrar la vista de actualizar préstamo
        public IActionResult ActualizarPrestamo()
        {
            var prestamo = _context.Prestamo.Where(p => p.Estado == "Activo").ToList();
            return View(prestamo);
        }

        // Acción para mostrar la vista de registrar devolución
        public IActionResult RegistrarDevolucion()
        {
            var prestamo = _context.Prestamo.Where(p => p.Estado == "Activo" || p.Estado == "Tardio").ToList();
            return View(prestamo);
        }

        // Acción para mostrar el formulario de registro de préstamo


        // Acción para registrar préstamo - POST
        public IActionResult ObtenerDatosSolicitud(int id)
        {
            var solicitud = _context.Solicitud
                .Include(s => s.Material)
                .Include(s => s.Alumno)
                .FirstOrDefault(s => s.IdSolicitud == id);

            if (solicitud == null)
            {
                return NotFound();
            }

            // Crear un objeto préstamo temporal para obtener las fechas
            var prestamoTemp = new Prestamo
            {
                FechaPrestamo = DateTime.Now,
                FechaDevolucion = DateTime.Now.AddDays(7) 
            };

            return Json(new
            {
                solicitud.IdSolicitud,
                solicitud.Alumno_Usuario_CodUsuario,
                solicitud.Material_CodMaterial,
                solicitud.Cantidad,
                prestamoTemp.FechaPrestamo,
                prestamoTemp.FechaDevolucion
            });
        }


        // Acción para registrar préstamo - POST
        [HttpPost]
        public async Task<IActionResult> RegistrarPrestamo(Prestamo prestamo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var connection = (MySqlConnection)_context.Database.GetDbConnection())
                    {
                        await connection.OpenAsync();

                        using (var command = new MySqlCommand("registrarPrestamo", connection))
                        {
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("p_solicitud_id", prestamo.Solicitud_IdSolicitud);
                            command.Parameters.AddWithValue("p_fecha_prestamo", DateTime.Now);

                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    return RedirectToAction(nameof(RegistrarPrestamo));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error al registrar el préstamo: {ex.Message}");
                }
            }
            return View(prestamo);
        }

        // Acción para actualizar préstamo - GET
        public IActionResult MostrarFormularioActualizar(int id)
        {
            var prestamo = _context.Prestamo.Find(id);
            if (prestamo == null)
            {
                return NotFound();
            }
            ViewBag.Action = "ActualizarPrestamo";
            return View("ActualizarPrestamo", prestamo);
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
            return View(prestamo);
        }

        // Acción para mostrar el formulario de registrar devolución
        public IActionResult MostrarFormularioDevolucion(int id)
        {
            var prestamo = _context.Prestamo.Find(id);
            if (prestamo == null)
            {
                return NotFound();
            }
            ViewBag.Action = "RegistrarDevolucion";
            return View("RegistrarDevolucion", prestamo);
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
            return View(prestamo);
        }
    }
}