using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Inventario.Models;
using Inventario.DTOs;
using Inventario.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Inventario.Controllers;
using Inventario.AutoMapperConfig;
using AutoMapper;

namespace Inventario.Controllers
{
    [Route("api/departamento")]
    public class DepartamentoController : Controller
    {
        private readonly ILogger<DepartamentoController> _logger;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        
        public DepartamentoController(ILogger<DepartamentoController> logger, DataContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }
        [HttpGet(Name = "GetDepartamentos"), AllowAnonymous]
        public async Task<ActionResult<PaginatedList<DepartamentoDTO>>> GetDepartamentos(int id, int pageNumber = 1, int pageSize = 6)
        {
            var datos = await _context.departamento.FindAsync(id);
            var allDepartamento = await _context.departamento.ToListAsync();
            var totalCount = allDepartamento.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            // var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var paginatedDepartamento = allDepartamento.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            var DepartamentosDTO = _mapper.Map<List<DepartamentoDTO>>(paginatedDepartamento);

            var paginatedList = new PaginatedList<DepartamentoDTO>
            {
                Items = DepartamentosDTO,
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
        public async Task<IActionResult> saveInformation([FromBody] DepartamentoDTO departamento)
        {
            if (ModelState.IsValid)
            {
                // Agregar el departamento al contexto y guardar los cambios en la base de datos
                Departamento newDepartamento = _mapper.Map<Departamento>(departamento);

                _context.departamento.Add(newDepartamento);
                await _context.SaveChangesAsync();

                // Devolver una respuesta CreatedAtRoute con el dispositivo creado
                return CreatedAtRoute("GetDepartamento", new { id = departamento.Id }, newDepartamento);
            }

            // Si el modelo no es válido, devolver una respuesta BadRequest
            return BadRequest(ModelState);
        }
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] DepartamentoDTO departamento)
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
                Departamento newDepartamento = _mapper.Map<Departamento>(departamento);
                _context.Update(newDepartamento);
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