using SistemaPrestamos.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using SistemaPrestamos.Context;
using SistemaPrestamos.Models;
using Microsoft.AspNetCore.Authorization;

namespace SistemaPrestamos.Controllers
{
    [Authorize]
    public class SolicitudesController : ActionUserController
    {
        private readonly ApplicationDbContext _context;

        public SolicitudesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult RegistrarSolicitud()
        {
            // Asegúrate de que el usuario está autenticado
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Security");
            }

            // Obtener el nombre del usuario autenticado
            var userName = User.Identity.Name;

            // Convertir userName a un entero
            if (int.TryParse(userName, out int userId))
            {
                // Obtener el usuario basado en el nombre de usuario
                var user = _context.Usuario.SingleOrDefault(u => u.CodUsuario == userId);
                if (user == null)
                {
                    // Manejar el caso cuando el usuario no se encuentra (esto no debería ocurrir normalmente)
                    return RedirectToAction("Login", "Security");
                }

                ViewBag.AlumnoCodUsuario = user.CodUsuario;

                // Inicializar materiales
                var materiales = _context.Material.ToList(); // Asegúrate de que Materiales existe en tu contexto y no es nulo
                ViewBag.Materiales = materiales;

                return View(materiales);
            }

            return RedirectToAction("Login", "Security");
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarSolicitud([FromBody] SolicitudDTO solicitud)
        {
            if (solicitud == null)
            {
                return BadRequest(new { message = "Datos de solicitud inválidos" });
            }

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "registrarSolicitud";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter("p_alumno_codUsuario", solicitud.alumnoCodUsuario));
                command.Parameters.Add(new MySqlParameter("p_material_cod", solicitud.materialCod));
                command.Parameters.Add(new MySqlParameter("p_cantidad", solicitud.cantidad));

                await _context.Database.OpenConnectionAsync();
                try
                {
                    await command.ExecuteNonQueryAsync();
                }
                catch (MySqlException ex)
                {
                    await _context.Database.CloseConnectionAsync();
                    return BadRequest(new { message = ex.Message });
                }
                finally
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }
            return Json(new { success = true });
        }

        public class SolicitudDTO
        {
            public int alumnoCodUsuario { get; set; }
            public int materialCod { get; set; }
            public int cantidad { get; set; }
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
                    return BadRequest(new { message = ex.Message });
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
