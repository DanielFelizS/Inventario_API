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
    [Route("api/computer")]
    public class PcController : Controller
    {
        private readonly ILogger<PcController> _logger;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        int number = 0;
        private readonly IWebHostEnvironment _host;
        
        public PcController(ILogger<PcController> logger, DataContext context, IMapper mapper, IWebHostEnvironment host)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _host = host;
        }
        [HttpGet(Name = "GetComputers"), AllowAnonymous]
        public async Task<ActionResult<PaginatedList<PCDTO>>> GetComputers(int id, int pageNumber = 1, int pageSize = 6)
        {

            var datos = await _context.Computer.FindAsync(id);

            var allComputer = await _context.Computer.ToListAsync();
            var totalCount = allComputer.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var paginatedComputer = allComputer.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            var ComputersDTO = _mapper.Map<List<PCDTO>>(paginatedComputer);

            var paginatedList = new PaginatedList<PCDTO>
            {
                Items = ComputersDTO,
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
        [HttpGet("{id}", Name = "GetComputer")]
        public async Task<ActionResult<PC>> GetComputer(int id)
        {
            var computer = await _context.Computer.FindAsync(id);
            if (computer == null)
            {
                return NotFound();
            }

            return computer;
        }
        [Authorize(Roles = StaticUserRoles.SOPORTE)]
        [HttpPost]
        public async Task<IActionResult> saveInformation([FromBody] PCDTO computer)
        {
            if (ModelState.IsValid)
            {
                // Agregar el departamento al contexto y guardar los cambios en la base de datos
                PC newComputer = _mapper.Map<PC>(computer);
                _context.Computer.Add(newComputer);
                await _context.SaveChangesAsync();

                // Devolver una respuesta CreatedAtRoute con el computer creado
                return CreatedAtRoute("GetComputer", new { id = computer.Id }, newComputer);
            }

            // Si el modelo no es válido, devolver una respuesta BadRequest
            return BadRequest(ModelState);
        }
        [Authorize(Roles = StaticUserRoles.SOPORTE)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] PCDTO computer)
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
                PC newComputer = _mapper.Map<PC>(computer);
                _context.Update(newComputer);
                await _context.SaveChangesAsync();
                return Ok("Se actualizó correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error mientras se actualizaban los datos: {ex.Message}");
            }
        }
        [Authorize(Roles = StaticUserRoles.SOPORTE)]
        [HttpDelete("{id}")]
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
        [AllowAnonymous]
        [HttpGet("reporte", Name = "GenerarReportePCs")]
        public async Task<IActionResult> DescargarPDF(int id)
        {
            var computer = await _context.Computer.ToListAsync();
            
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
                            col2.Item().Text("Computadoras").Underline().Bold();

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
                            });
                            tabla.Header(header =>
                            {
                                header.Cell().Background("#257272").Text("Id").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("Nombre").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("RAM").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("Disco Duro").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("Procesador").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("Ventilador").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("Fuente de Poder").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("MotherBoard").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("Tipo de MotherBoard").FontColor("#fff").FontSize(10);
                            });

                            // var dispositivos = new List<DispositivoDTO>();
                            // var dispositivos = await _context.Dispositivos.ToListAsync();

                            // Console.WriteLine("Cantidad de dispositivos: " + dispositivos.Count);
                            
                            foreach (var Computer in computer)
                            {
                                var id = Computer.Id;
                                var nombre = Computer.Equipo_Id;
                                var ram = Computer.RAM;
                                var discoDuro = Computer.Disco_duro;
                                var procesador = Computer.Procesador;
                                var ventilador = Computer.Ventilador;
                                var fuentePoder = Computer.FuentePoder;
                                var motherBoard = Computer.MotherBoard;
                                var tipoMotherBoard = Computer.Tipo_MotherBoard;

                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(id).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(nombre).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(ram).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(discoDuro).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(procesador).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(ventilador).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(fuentePoder).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(motherBoard).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(tipoMotherBoard).FontSize(10);
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
            return File(stream, "application/pdf", "detallecomputadoras.pdf");
        }
    }
}