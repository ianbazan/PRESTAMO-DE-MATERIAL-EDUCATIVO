using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPrestamos.Models;
using SistemaPrestamos.Context;
using System;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnector;
using SistemaPrestamos.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace SistemaPrestamos.Controllers
{
    [Authorize(Roles = "Encargado")]
    public class PrestamosController : ActionUserController
    {
        private readonly ApplicationDbContext _context;

        public PrestamosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public void ActualizarEstadoPrestamosVencidos()
        {
            var prestamosVencidos = _context.Prestamo
                .Where(p => p.Estado == "En Curso" && p.FechaPrestamo.AddDays(7) < DateTime.Now)
                .ToList();

            foreach (var prestamo in prestamosVencidos)
            {
                prestamo.Estado = "Tardio";
            }

            if (prestamosVencidos.Any())
            {
                _context.SaveChanges();
            }
        }

        public IActionResult RegistrarPrestamo()
        {
            var solicitudes = _context.Solicitud.Where(s => s.Estado == "Generado").ToList();
            return View(solicitudes);
        }

        public IActionResult ActualizarPrestamo()
        {
            ActualizarEstadoPrestamosVencidos();
            var prestamos = _context.Prestamo
                .Where(p => p.Estado == "En Curso" || p.Estado == "Activo")
                .Include(p => p.Solicitud)
                .ToList();
            return View(prestamos);
        }
        public IActionResult RegistrarDevolucion()
        {
            ActualizarEstadoPrestamosVencidos(); // Asegúrate de que se actualizan los estados de los préstamos vencidos
            var prestamos = _context.Prestamo
                .Where(p => p.Estado == "En curso" || p.Estado == "Tardio")
                .Include(p => p.Solicitud) // Incluir la entidad Solicitud
                .ToList();
            return View(prestamos);
        }

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

            var fechaPrestamo = DateTime.Now;
            var fechaDevolucion = fechaPrestamo.AddDays(7);

            return Json(new
            {
                solicitud.IdSolicitud,
                solicitud.Alumno_Usuario_CodUsuario,
                solicitud.Material_CodMaterial,
                solicitud.Cantidad,
                FechaPrestamo = fechaPrestamo.ToString("yyyy-MM-ddTHH:mm:ss"), // Fecha actual
                FechaDevolucion = fechaDevolucion.ToString("yyyy-MM-dd") // Calculado en el código
            });
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarPrestamo(Prestamo prestamo)
        {
            if (prestamo == null)
            {
                return BadRequest(new { message = "Datos de préstamo inválidos" });
            }

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "registrarPrestamo";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter("p_solicitud_id", prestamo.Solicitud_IdSolicitud));
                command.Parameters.Add(new MySqlParameter("p_fecha_prestamo", DateTime.Now));

                await _context.Database.OpenConnectionAsync();
                try
                {
                    await command.ExecuteNonQueryAsync();
                    ActualizarEstadoPrestamosVencidos(); // Llamar al método aquí
                }
                catch (MySqlException ex)
                {
                    await _context.Database.CloseConnectionAsync();
                    ModelState.AddModelError(string.Empty, $"Error al registrar el préstamo: {ex.Message}");
                    return View(prestamo);
                }
                finally
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }

            return RedirectToAction(nameof(RegistrarPrestamo));
        }

        public IActionResult ObtenerDatosPrestamo(int id)
        {
            var prestamo = _context.Prestamo
                .Include(p => p.Solicitud.Material)
                .Include(p => p.Solicitud.Alumno)
                .FirstOrDefault(p => p.IdPrestamo == id);

            if (prestamo == null)
            {
                return NotFound();
            }

            var fechaPrestamo = prestamo.FechaPrestamo;
            var fechaDevolucion = fechaPrestamo.AddDays(7);
            var fechaRealDevolucion = prestamo.Fecha_Dev_Real ?? DateTime.Now; // Use the actual return date if available, otherwise use current date

            return Json(new
            {
                prestamo.IdPrestamo,
                prestamo.Solicitud.Alumno_Usuario_CodUsuario,
                prestamo.Solicitud.Material_CodMaterial,
                prestamo.Solicitud.Cantidad,
                FechaPrestamo = fechaPrestamo.ToString("yyyy-MM-ddTHH:mm:ss"), // Original loan date
                FechaDevolucion = fechaDevolucion.ToString("yyyy-MM-dd"), // Scheduled return date
                FechaRealDevolucion = fechaRealDevolucion.ToString("yyyy-MM-ddTHH:mm:ss") // Actual return date or current date
            });
        }


        [HttpPost]
        public async Task<IActionResult> ActualizarPrestamo(Prestamo prestamo)
        {
            if (prestamo == null)
            {
                return BadRequest(new { message = "Datos de préstamo inválidos" });
            }

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "actualizarPrestamo";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter("p_prestamo_id", prestamo.IdPrestamo));
                command.Parameters.Add(new MySqlParameter("p_nueva_fecha_prestamo", DateTime.Now));

                await _context.Database.OpenConnectionAsync();
                try
                {
                    await command.ExecuteNonQueryAsync();
                    ActualizarEstadoPrestamosVencidos();
                }
                catch (MySqlException ex)
                {
                    await _context.Database.CloseConnectionAsync();
                    ModelState.AddModelError(string.Empty, $"Error al actualizar el préstamo: {ex.Message}");
                    return View(prestamo);
                }
                finally
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }

            return RedirectToAction(nameof(ActualizarPrestamo));
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarDevolucion(Prestamo prestamo)
        {
            if (prestamo == null)
            {
                return BadRequest(new { message = "Datos de préstamo inválidos" });
            }

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "registrarDevolucion";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter("p_prestamo_id", prestamo.IdPrestamo));
                command.Parameters.Add(new MySqlParameter("p_fecha_devolucion", DateTime.Now));

                await _context.Database.OpenConnectionAsync();
                try
                {
                    await command.ExecuteNonQueryAsync();
                }
                catch (MySqlException ex)
                {
                    await _context.Database.CloseConnectionAsync();
                    ModelState.AddModelError(string.Empty, $"Error al registrar la devolucion: {ex.Message}");
                    return View(prestamo);
                }
                finally
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }

            return RedirectToAction(nameof(RegistrarDevolucion));
        }
    }
}
