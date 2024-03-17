using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Inventario.Models;
using Inventario.DTOs;
using Inventario.Data;
using Inventario.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Inventario.Controllers;
using Inventario.AutoMapperConfig;
using AutoMapper;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Previewer;
using System.IO;

namespace Inventario.Controllers
{
    [Route("api/dispositivos")]
    public class DispositivoController : Controller
    {
        private readonly ILogger<DispositivoController> _logger;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        int number = 0;
        private readonly IWebHostEnvironment _host;
        
        public DispositivoController(ILogger<DispositivoController> logger, DataContext context, IMapper mapper, IWebHostEnvironment host)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _host = host;

        }
        [AllowAnonymous]
        [HttpGet(Name = "GetDispositivos")]
        public async Task<ActionResult<PaginatedList<DispositivoDTO>>> GetDispositivos(int id, int pageNumber = 1, int pageSize = 6)
        {
            var paginatedDispositivos = await (from dispositivo in _context.Dispositivos
                join departamento in _context.departamento on dispositivo.DepartamentoId equals departamento.Id
                select new
                {
                    Dispositivo = dispositivo,
                    Nombre_departamento = departamento.Nombre
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await _context.Dispositivos.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Mapear la lista de dispositivos a una lista de DispositivoDTO
            var dispositivosDTO = paginatedDispositivos.Select(d =>
            {
                var dispositivoDTO = _mapper.Map<DispositivoDTO>(d.Dispositivo);
                dispositivoDTO.Nombre_departamento = d.Nombre_departamento;
                return dispositivoDTO;
            }).ToList();

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
        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<ActionResult<PaginatedList<DispositivoDTO>>> Search(int id, int pageNumber = 1, int pageSize = 6, string search = null)
        {
            // Consulta inicial de dispositivos
            IQueryable<Dispositivo> consulta = _context.Dispositivos.Include(d => d.departamento);

            // Filtrar por búsqueda si se proporciona
            if (!string.IsNullOrEmpty(search))
            {
                consulta = consulta.Where(d => d.Nombre_equipo.Contains(search));
            }

            // Realizar la consulta paginada
            var paginatedDispositivos = await consulta
                .Select(d => new
                {
                    Dispositivo = d,
                    Nombre_departamento = d.departamento.Nombre
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await consulta.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Mapear la lista de dispositivos a una lista de DispositivoDTO
            var dispositivosDTO = paginatedDispositivos.Select(d =>
            {
                var dispositivoDTO = _mapper.Map<DispositivoDTO>(d.Dispositivo);
                dispositivoDTO.Nombre_departamento = d.Nombre_departamento;
                return dispositivoDTO;
            }).ToList();

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
        [Authorize(Roles = StaticUserRoles.SOPORTE)]
        [HttpGet("{id}", Name = "GetDispositivoBy"), Authorize]
        public async Task<ActionResult<Dispositivo>> GetDispositivo(int id)
        {
            var dispositivo = await _context.Dispositivos.FindAsync(id);
            if (dispositivo == null)
            {
                return NotFound();
            }

            return dispositivo;
        }
        [Authorize(Roles = StaticUserRoles.SOPORTE)]
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
        [Authorize(Roles = StaticUserRoles.SOPORTE)]
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
        [Authorize(Roles = StaticUserRoles.SOPORTE)]
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
        [AllowAnonymous]
        [HttpGet("reporte", Name = "GenerarReporteDispositivos")]
        public async Task<IActionResult> DescargarPDF(int id)
        {
            var dispositivos = await _context.Dispositivos.ToListAsync();
            
            var data = Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(30);

                    page.Header().ShowOnce().Row(row =>
                    {
                        var rutaImagen = Path.Combine(_host.WebRootPath, "images/MIVED.png");
                        byte[] imageData = System.IO.File.ReadAllBytes(rutaImagen);
                         //row.ConstantItem(140).Height(60).Placeholder();
                        row.ConstantItem(150).Image(imageData);
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().AlignCenter().Text("Ministerio de vivienda y edificaciones").Bold().FontSize(12);
                            col.Item().AlignCenter().Text("Avenida Pedro Henríquez Ureña 124, Esquina Av. Alma Mater, Santo Domingo 10107, República Dominicana").FontSize(8);
                            col.Item().AlignCenter().Text("https://mived.gob.do/").FontSize(8);
                            col.Item().AlignCenter().Text("codigo@example.com").FontSize(8);
                        });
                    });

                    page.Content().PaddingVertical(10).Column(col1 =>
                    {
                        col1.Item().Column(col2 =>
                        {
                            col2.Item().Text("Datos del empleado").Underline().Bold();

                            col2.Item().Text(txt =>
                            {
                                txt.Span("Nombre: ").SemiBold().FontSize(10);
                                txt.Span("Faustino Acevedo").FontSize(10);
                            });

                            col2.Item().Text(txt =>
                            {
                                txt.Span("Correo: ").SemiBold().FontSize(10);
                                txt.Span("fautino.acevedo@mived.gob.do").FontSize(10);
                            });

                            col2.Item().Text(txt =>
                            {
                                txt.Span("Departamento: ").SemiBold().FontSize(10);
                                txt.Span("Tecnología").FontSize(10);
                            });
                            col2.Item().Text("Dispositivos").Underline().Bold();

                        });

                        col1.Item().LineHorizontal(0.5f);
                        col1.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(6);
                                columns.RelativeColumn(6);
                                columns.RelativeColumn(6);
                                columns.RelativeColumn(6);
                                columns.RelativeColumn(6);
                                columns.RelativeColumn(6);
                                columns.RelativeColumn(6);
                                columns.RelativeColumn(6);
                                columns.RelativeColumn(6);
                                columns.RelativeColumn(6);
                            });
                            tabla.Header(header =>
                            {
                                header.Cell().Background("#257272").Text("Id").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("Nombre").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("Marca").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("Modelo").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("Estado").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("Serial").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("Invi").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("Bienes").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("Fecha").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("Propietario").FontColor("#fff").FontSize(10);
                            });

                            // var dispositivos = new List<DispositivoDTO>();
                            // var dispositivos = await _context.Dispositivos.ToListAsync();

                            // Console.WriteLine("Cantidad de dispositivos: " + dispositivos.Count);
                            foreach (var dispositivo in dispositivos)
                            {
                                var id = dispositivo.Id;
                                var nombre = dispositivo.Nombre_equipo;
                                var marca = dispositivo.Marca;
                                var modelo = dispositivo.Modelo;
                                var estado = dispositivo.Estado;
                                var noSerial = dispositivo.Serial_no;
                                var invi = dispositivo.Cod_inventario;
                                var bienes = dispositivo.Bienes_nacionales.ToString();
                                var fecha = dispositivo.Fecha_modificacion?.ToString("dd-MM-yy");
                                var propietario = dispositivo.Propietario_equipo;

                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(id).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(nombre).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(marca).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(modelo).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(estado).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(noSerial).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(invi).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(bienes).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(fecha).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(propietario).FontSize(10);
                            }
                        });
                    });

                    page.Footer()
                    .AlignRight()
                    .Text(txt =>
                    {
                        txt.Span("Pagina ").FontSize(10);
                        txt.CurrentPageNumber().FontSize(10);
                        txt.Span(" de ").FontSize(10);
                        txt.TotalPages().FontSize(10);
                    });
                });
            }).GeneratePdf();

            Stream stream = new MemoryStream(data);
            return File(stream, "application/pdf", "detalledispositivos.pdf");
        }
    }
}