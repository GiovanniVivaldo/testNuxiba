using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using testNuxiba.Data;
using testNuxiba.Models;

namespace testNuxiba.Controllers
{
    [ApiController]
    [Route("logins")]
    public class LoginController : ControllerBase
    {
        private readonly dbContext _context;

        public LoginController(dbContext context)
        {
            _context = context; 
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Login>>> GetLogins()
        {
            return await _context.ccLogLogin.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<string>> ToggleLogin()
        {
            var now = DateTime.Now;

            // 1. Buscar al último usuario que esté logueado (último movimiento = login sin logout)
            var lastLogin = await _context.ccLogLogin
                .OrderByDescending(l => l.Fecha)
                .FirstOrDefaultAsync();

            if (lastLogin != null && lastLogin.TipoMov == 1)
            {
                // El último movimiento fue un login entonces se desloguea
                var logout = new Login
                {
                    User_id = lastLogin.User_id,
                    TipoMov = 0,
                    Fecha = now,
                    Extension = lastLogin.Extension
                };

                _context.ccLogLogin.Add(logout);
                await _context.SaveChangesAsync();

                return Ok($"Se deslogueó al usuario con ID {logout.User_id}.");
            }
            else
            {
                // No hay login activo → loguear a un usuario aleatorio
                var user = await _context.ccUsers.OrderBy(u => Guid.NewGuid()).FirstOrDefaultAsync();
                if (user == null)
                    return NotFound("No hay usuarios disponibles para loguear.");

                var login = new Login
                {
                    User_id = user.User_id,
                    TipoMov = 1,
                    Fecha = now,
                    Extension = new Random().Next(-10, 11)
                };

                _context.ccLogLogin.Add(login);
                await _context.SaveChangesAsync();

                return Ok($"Se logueó al usuario: {user.User_id}.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutLogin(int id)
        {
            // Busca el registro más reciente por User_id (id = User_id aquí)
            var existing = await _context.ccLogLogin
                .Where(l => l.User_id == id)
                .OrderByDescending(l => l.Fecha)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                return NotFound($"No se encontró un login/deslogin para el usuario con ID {id}.");
            }

            // Actualiza la fecha a la actual
            existing.Fecha = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = $"El registro más reciente del usuario {id} se actualizó a la hora actual.",
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLogin(int id)
        {
            var login = await _context.ccLogLogin.FindAsync(id);
            if (login == null)
            {
                return NotFound("Registro no encontrado.");
            }

            _context.ccLogLogin.Remove(login);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = $"Registro eliminado.",
            });
        }

        [HttpGet("csv")]
        public async Task<IActionResult> GetCsvReport()
        {
            // Obtener todos los movimientos ordenados
            var movimientos = await _context.ccLogLogin
                .OrderBy(m => m.User_id)
                .ThenBy(m => m.Fecha)
                .ToListAsync();

            var sesiones = new List<(int UserId, TimeSpan Duracion)>();

            // Emparejar login con logout por usuario
            foreach (var grupo in movimientos.GroupBy(m => m.User_id))
            {
                var lista = grupo.ToList();
                for (int i = 0; i < lista.Count - 1; i++)
                {
                    if (lista[i].TipoMov == 1) // login
                    {
                        var logout = lista.Skip(i + 1).FirstOrDefault(x => x.TipoMov == 0 && x.Fecha > lista[i].Fecha);
                        if (logout != null)
                        {
                            sesiones.Add((lista[i].User_id, logout.Fecha - lista[i].Fecha));
                            i = lista.IndexOf(logout); // saltar al logout
                        }
                    }
                }
            }

            // Agrupar y sumar duración por usuario
            var resumen = sesiones
                .GroupBy(s => s.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    TotalHoras = g.Sum(x => x.Duracion.TotalHours)
                })
                .ToList();

            // Obtener info de usuarios + área
            var datosFinales = await _context.ccUsers
                .Include(u => u.Area)
                .Select(u => new
                {
                    u.User_id,
                    u.Login,
                    NombreCompleto = u.Nombres + " " + u.ApellidoPaterno + " " + u.ApellidoMaterno,
                    Area = u.Area != null ? u.Area.AreaName : "",
                })
                .ToListAsync();

            // Generar CSV en memoria
            var csv = new StringBuilder();
            csv.AppendLine("Login,Nombre Completo,Area,TotalHoras");

            foreach (var usuario in datosFinales)
            {
                var total = resumen.FirstOrDefault(x => x.UserId == usuario.User_id)?.TotalHoras ?? 0;
                csv.AppendLine($"{usuario.Login},{usuario.NombreCompleto},{usuario.Area},{total:F2}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "reporte_trabajo.csv");
        }

    }
}
