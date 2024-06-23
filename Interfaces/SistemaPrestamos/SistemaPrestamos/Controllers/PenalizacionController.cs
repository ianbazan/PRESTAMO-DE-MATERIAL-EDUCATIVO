using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using SistemaPrestamos.Context;
using SistemaPrestamos.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaPrestamos.Controllers
{
    public class PenalizacionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PenalizacionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Acción para mostrar préstamos para registrar penalización
        [HttpGet]
        public IActionResult RegistrarPenalizacion()
        {
            var prestamos = _context.Prestamo
                .Include(p => p.Solicitud)
                .ThenInclude(s => s.Alumno)
                .Where(p => p.Estado == "Activo" || p.Estado == "Tardío")
                .ToList();
            return View(prestamos);
        }

        // Acción para registrar penalización
        [HttpPost]
        public async Task<IActionResult> RegistrarPenalizacion(int prestamoId, string descripcion)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "registrarPenalizacion";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter("p_prestamo_id", prestamoId));
                command.Parameters.Add(new MySqlParameter("p_fecha_penalizacion", DateTime.Now));
                command.Parameters.Add(new MySqlParameter("p_descripcion", descripcion));

                await _context.Database.OpenConnectionAsync();
                try
                {
                    await command.ExecuteNonQueryAsync();

                    // Modificar el estado del préstamo a "Penalizado"
                    var prestamo = await _context.Prestamo.FindAsync(prestamoId);
                    if (prestamo != null)
                    {
                        prestamo.Estado = "Penalizado";
                        _context.Update(prestamo);
                        await _context.SaveChangesAsync();
                    }
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

            return Ok(new { message = "Se ha registrado la penalización con éxito." });
        }

        // Acción para registrar penalización fuera de plazo
        [HttpPost]
        public async Task<IActionResult> PenalizacionFueraDePlazo(int prestamoId, int diasExcedidos)
        {
            var prestamo = await _context.Prestamo
                .Include(p => p.Solicitud)
                .ThenInclude(s => s.Alumno)
                .FirstOrDefaultAsync(p => p.IdPrestamo == prestamoId);

            if (prestamo == null || prestamo.Solicitud == null || prestamo.Solicitud.Alumno == null)
            {
                return NotFound();
            }

            // Modificar el estado del préstamo a "Penalizado"
            prestamo.Estado = "Penalizado";
            _context.Update(prestamo);

            // Modificar el estado del alumno a "Inhabilitado"
            var alumno = prestamo.Solicitud.Alumno;
            alumno.Estado = "Inhabilitado";
            // Suponiendo que existe una propiedad para los días de inhabilitación
            alumno.DiasInhabilitado += diasExcedidos;
            _context.Update(alumno);

            await _context.SaveChangesAsync();

            return Ok(new { message = "PRÉSTAMO FUERA DE PLAZO. Alumno inhabilitado por " + diasExcedidos + " días." });
        }
    }
}
