using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Inventario.Models;
using Inventario.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Inventario.Controllers;

namespace Inventario.Controllers
{
    [Route("api/departamento")]
    public class DepartamentoController : Controller
    {
        private readonly ILogger<DepartamentoController> _logger;
        private readonly DataContext _context;
        
        public DepartamentoController(ILogger<DepartamentoController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet(Name = "GetDepartamentos"), AllowAnonymous]
        public async Task<ActionResult<PaginatedList<Departamento>>> GetDepartamentos(int id, int pageNumber = 1, int pageSize = 6)
        {
            var datos = await _context.Dispositivos.FindAsync(id);
            var allDepartamento = await _context.departamento.ToListAsync();
            var totalCount = allDepartamento.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            // var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var paginatedDepartamento = allDepartamento.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedList = new PaginatedList<Departamento>
            {
                Items = paginatedDepartamento,
                TotalCount = totalCount,
                PageIndex = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            // Agrega el encabezado 'X-Total-Count' a la respuesta
            Response.Headers["X-Total-Count"] = totalCount.ToString();
            // Exponer el encabezado 'X-Total-Count'
            Response.Headers.Append("Access-Control-Expose-Headers", "X-Total-Count");
            
            return paginatedList;
        }
        [HttpGet("{id}", Name = "GetDepartamento"), Authorize]
        public async Task<ActionResult<Departamento>> GetDepartamento(int id)
        {
            var departamento = await _context.departamento.FindAsync(id);

            if (departamento == null)
            {
                return NotFound();
            }

            return departamento;
        }
        [HttpPost, Authorize]
        public async Task<IActionResult> saveInformation([FromBody] Departamento departamento)
        {
            if (ModelState.IsValid)
            {
                // Agregar el departamento al contexto y guardar los cambios en la base de datos
                _context.departamento.Add(departamento);
                await _context.SaveChangesAsync();

                // Devolver una respuesta CreatedAtRoute con el dispositivo creado
                return CreatedAtRoute("GetDepartamento", new { id = departamento.Id }, departamento);
            }

            // Si el modelo no es válido, devolver una respuesta BadRequest
            return BadRequest(ModelState);
        }
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] Departamento departamento)
        {
            if (id != departamento?.Id)
            {
                return BadRequest("No se encontró el ID");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Update(departamento);
                await _context.SaveChangesAsync();
                return Ok("Se actualizó correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error mientras se actualizaban los datos: {ex.Message}");
            }
        }
        [HttpDelete("{id}"), Authorize]
        public async Task<ActionResult<Departamento>> Delete(int id)
        {
            var departamento = await _context.departamento.FindAsync(id);

            if (departamento== null)
            {
                return NotFound();
            }

            _context.departamento.Remove(departamento);
            await _context.SaveChangesAsync();

            return departamento;
        }
    }
}