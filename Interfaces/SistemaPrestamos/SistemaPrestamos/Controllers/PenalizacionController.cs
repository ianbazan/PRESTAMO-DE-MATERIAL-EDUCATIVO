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
                .Where(p => p.Estado == "En Curso" || p.Estado == "Tardío")
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

            var fechaPrestamo = DateTime.Now;
            var fechaDevolucion = fechaPrestamo.AddDays(7);

            return Json(new
            {
                prestamo.IdPrestamo,
                prestamo.Solicitud.Alumno_Usuario_CodUsuario,
                prestamo.Solicitud.Material_CodMaterial,
                prestamo.Solicitud.Cantidad,
                FechaPrestamo = fechaPrestamo.ToString("yyyy-MM-ddTHH:mm:ss"), // Fecha actual
                FechaDevolucion = fechaDevolucion.ToString("yyyy-MM-dd") // Fecha de devolución calculada
            });
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

            return Ok(new { message = "Se ha registrado la penalización con éxito.", redirectUrl = Url.Action("RegistrarPenalizacion", "Penalizacion") } );

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
