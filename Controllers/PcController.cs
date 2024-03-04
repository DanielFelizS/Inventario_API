using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Inventario.Models;
using Inventario.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Inventario.Controllers;

namespace Inventario.Controllers
{
    [Route("api/computer")]
    public class PcController : Controller
    {
        private readonly ILogger<PcController> _logger;
        private readonly DataContext _context;
        
        public PcController(ILogger<PcController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet(Name = "GetComputers"), AllowAnonymous]
        public async Task<ActionResult<PaginatedList<PC>>> GetComputers(int id, int pageNumber = 1, int pageSize = 6)
        {

            var datos = await _context.Computer.FindAsync(id);

            var allComputer = await _context.Computer.ToListAsync();
            var totalCount = allComputer.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var paginatedComputer = allComputer.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            var paginatedList = new PaginatedList<PC>
            {
                Items = paginatedComputer,
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
        [HttpGet("{id}", Name = "GetComputer"), Authorize]
        public async Task<ActionResult<PC>> GetComputer(int id)
        {
            var computer = await _context.Computer.FindAsync(id);
            if (computer == null)
            {
                return NotFound();
            }

            return computer;
        }
        [HttpPost, Authorize]
        public async Task<IActionResult> saveInformation([FromBody] PC computer)
        {
            if (ModelState.IsValid)
            {
                // Agregar el departamento al contexto y guardar los cambios en la base de datos
                _context.Computer.Add(computer);
                await _context.SaveChangesAsync();

                // Devolver una respuesta CreatedAtRoute con el computer creado
                return CreatedAtRoute("GetComputer", new { id = computer.Id }, computer);
            }

            // Si el modelo no es válido, devolver una respuesta BadRequest
            return BadRequest(ModelState);
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] PC computer)
        {
            if (id != computer?.Id)
            {
                return BadRequest("No se encontró el ID");
            }

            else if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Update(computer);
                await _context.SaveChangesAsync();
                return Ok("Se actualizó correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error mientras se actualizaban los datos: {ex.Message}");
            }
        }
        [HttpDelete("{id}"), Authorize]
        public async Task<ActionResult<PC>> Delete(int id)
        {
            var computer = await _context.Computer.FindAsync(id);
            if (computer == null)
            {
                return NotFound();
            }

            _context.Computer.Remove(computer);
            await _context.SaveChangesAsync();

            return computer;
        }
    }
}