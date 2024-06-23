using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using SistemaPrestamos.Context;
using SistemaPrestamos.Models;

namespace SistemaPrestamos.Controllers
{
    public class SolicitudesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SolicitudesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult RegistrarSolicitud()
        {
            var materiales = _context.Material.ToList();
            return View(materiales);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarSolicitud(int alumnoCodUsuario, int materialCod, int cantidad)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "registrarSolicitud";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter("p_alumno_id", alumnoCodUsuario));
                command.Parameters.Add(new MySqlParameter("p_material_cod", materialCod));
                command.Parameters.Add(new MySqlParameter("p_cantidad", cantidad));

                await _context.Database.OpenConnectionAsync();
                try
                {
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    // Manejo de errores
                    return BadRequest(ex.Message);
                }
                finally
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarEstadoSolicitud(int solicitudId, string nuevoEstado)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "actualizarEstadoSolicitud";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter("p_solicitud_id", solicitudId));
                command.Parameters.Add(new MySqlParameter("p_nuevo_estado", nuevoEstado));

                await _context.Database.OpenConnectionAsync();
                try
                {
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    // Manejo de errores
                    return BadRequest(ex.Message);
                }
                finally
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }

            return Ok();
        }
    }
}
