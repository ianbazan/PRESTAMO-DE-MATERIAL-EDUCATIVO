using SistemaPrestamos.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using SistemaPrestamos.Context;
using SistemaPrestamos.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SistemaPrestamos.Controllers
{
    [Authorize(Roles = "Encargado")]
    public class PenalizacionController : ActionUserController
    {
        private readonly ApplicationDbContext _context;

        public PenalizacionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult RegistrarPenalizacion()
        {
            var prestamos = _context.Prestamo
                .Include(p => p.Solicitud)
                .Where(p => p.Estado == "En Curso" || p.Estado == "Tardio")
                .ToList();
            return View(prestamos);
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

            return Json(new
            {
                prestamo.IdPrestamo,
                prestamo.Solicitud.Alumno_Usuario_CodUsuario,
                prestamo.Solicitud.Material_CodMaterial,
                prestamo.Solicitud.Cantidad,
                prestamo.Estado,
                FechaPenalizacion = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
            });
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarPenalizacion(int IdPrestamo, string Descripcion)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "registrarPenalizacion";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter("p_prestamo_id", IdPrestamo));
                command.Parameters.Add(new MySqlParameter("p_fecha_penalizacion", DateTime.Now));
                command.Parameters.Add(new MySqlParameter("p_descripcion", Descripcion));

                await _context.Database.OpenConnectionAsync();
                try
                {
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                finally
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }

            return Ok(new { message = "Se ha registrado la penalización con éxito.", redirectUrl = Url.Action("RegistrarPenalizacion", "Penalizacion") });
        }
    }
}
