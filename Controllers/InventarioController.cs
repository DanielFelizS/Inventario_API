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
    [Route("api/dispositivos")]
    public class DispositivoController : Controller
    {
        private readonly ILogger<DispositivoController> _logger;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        
        public DispositivoController(ILogger<DispositivoController> logger, DataContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }
        [AllowAnonymous]
        [HttpGet(Name = "GetDispositivos")]
        public async Task<ActionResult<PaginatedList<DispositivoDTO>>> GetDispositivos(int id, int pageNumber = 1, int pageSize = 6)
        {
            var allDispositivos = await _context.Dispositivos.ToListAsync();
            var totalCount = allDispositivos.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var paginatedDispositivos = allDispositivos.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Mapear la lista de dispositivos a una lista de DispositivoDTO
            var dispositivosDTO = _mapper.Map<List<DispositivoDTO>>(paginatedDispositivos);

            var paginatedList = new PaginatedList<DispositivoDTO>
            {
                Items = dispositivosDTO,
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
        [HttpGet("{id}", Name = "GetDispositivo"), Authorize]
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
        public async Task<IActionResult> saveInformation([FromBody] DispositivoDTO dispositivo)
        {
            if (ModelState.IsValid)
            {
                Dispositivo newDispositivo = _mapper.Map<Dispositivo>(dispositivo);
                // Agregar el departamento al contexto y guardar los cambios en la base de datos
                _context.Dispositivos.AddAsync(newDispositivo);
                await _context.SaveChangesAsync();

                // Devolver una respuesta CreatedAtRoute con el dispositivo creado
                return CreatedAtRoute("Getdispositivo", new { id = newDispositivo.Id }, newDispositivo);
            }

            // Si el modelo no es válido, devolver una respuesta BadRequest
            return BadRequest(ModelState);
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] DispositivoDTO dispositivo)
        {
            if (id != dispositivo?.Id)
            {
                return BadRequest("No se encontró el ID");
            }

            else if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Dispositivo newDispositivo = _mapper.Map<Dispositivo>(dispositivo);
                _context.Update(newDispositivo);
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