using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Inventario.Models;
using Inventario.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Inventario.Controllers;

namespace Inventario.Controllers
{
    [Route("api/dispositivos")]
    public class DispositivoController : Controller
    {
        private readonly ILogger<DispositivoController> _logger;
        private readonly DataContext _context;

        public DispositivoController(ILogger<DispositivoController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        [AllowAnonymous]
        [HttpGet(Name = "GetDispositivos")]
        public async Task<ActionResult<IEnumerable<Dispositivo>>> GetDispositivos()
        {
            var dispositivos = await _context.Dispositivos.ToListAsync();

            // Calcula el total de elementos
            var total = dispositivos.Count;

            // Agrega el encabezado 'X-Total-Count' a la respuesta
            // Response.Headers.Append("X-Total-Count", total.ToString());
            // o
            Response.Headers["X-Total-Count"] = total.ToString();

            // Exponer el encabezado 'X-Total-Count'
            Response.Headers.Append("Access-Control-Expose-Headers", "X-Total-Count");

            return dispositivos;
        }

        [AllowAnonymous]
        [HttpGet("{id}", Name = "GetDispositivo")]
        public async Task<ActionResult<Dispositivo>> GetDispositivo(int id)
        {
            var dispositivo = await _context.Dispositivos.FindAsync(id);

            if (dispositivo == null)
            {
                return NotFound();
            }

            return dispositivo;
        }
        [HttpPost, Authorize]
        public async Task<IActionResult> saveInformation([FromBody] Dispositivo dispositivo)
        {
            if (ModelState.IsValid)
            {
                // Agregar el dispositivo al contexto y guardar los cambios en la base de datos
                _context.Dispositivos.Add(dispositivo);
                await _context.SaveChangesAsync();

                // Devolver una respuesta CreatedAtRoute con el dispositivo creado
                return CreatedAtRoute("GetDispositivo", new { id = dispositivo.Id }, dispositivo);
            }

            // Si el modelo no es válido, devolver una respuesta BadRequest
            return BadRequest(ModelState);
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] Dispositivo dispositivo)
        {
            if (id != dispositivo?.Id)
            {
                return BadRequest("No se encontró el ID");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Update(dispositivo);
                await _context.SaveChangesAsync();
                return Ok("Se actualizó correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error mientras se actualizaban los datos: {ex.Message}");
            }
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<ActionResult<Dispositivo>> Delete(int id)
        {
            var dispositivo = await _context.Dispositivos.FindAsync(id);

            if (dispositivo == null)
            {
                return NotFound();
            }

            _context.Dispositivos.Remove(dispositivo);
            await _context.SaveChangesAsync();

            return dispositivo;
        }
    }
}