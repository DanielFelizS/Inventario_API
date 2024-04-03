using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Inventario.Models;
using Inventario.DTOs;
using Inventario.Data;
using Inventario.Authorization;
using AutoMapper;
using QuestPDF.Fluent;
using OfficeOpenXml;
using ClosedXML.Excel;

namespace Inventario.Controllers
{
    [Route("api/departamento")]
    public class DepartamentoController : Controller
    {
        private readonly ILogger<DepartamentoController> _logger;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _host;
        private readonly AuditoriaService _auditoriaService;
        int number = 0;

        public DepartamentoController(ILogger<DepartamentoController> logger, DataContext context, IMapper mapper,  IWebHostEnvironment host, AuditoriaService auditoriaService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _host = host;
            _auditoriaService = auditoriaService;      
        }
        // [Authorize(Roles = StaticUserRoles.ADMIN + "," + StaticUserRoles.SUPERADMIN)]
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
        [HttpGet("search"), AllowAnonymous]
        public async Task<ActionResult<PaginatedList<DepartamentoDTO>>> Search(int id, int pageNumber = 1, int pageSize = 6, string search = null)
        {
            var consulta = _context.departamento.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                consulta = consulta.Where(d => d.Nombre != null && d.Nombre.Contains(search) ||
                d.Fecha_creacion.ToString() != null && d.Fecha_creacion.ToString().Contains(search) ||
                d.Encargado != null && d.Encargado.Contains(search));
            }

            var totalCount = await consulta.CountAsync();

            // Obtener los dispositivos paginados
            var paginatedDispositivos = await consulta
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var departamentosDto = _mapper.Map<List<DepartamentoDTO>>(paginatedDispositivos);

            var paginatedList = new PaginatedList<DepartamentoDTO>
            {
                Items = departamentosDto,
                TotalCount = totalCount,
                PageIndex = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            // Agregar el encabezado 'X-Total-Count' a la respuesta
            Response.Headers["X-Total-Count"] = paginatedList.TotalCount.ToString();
            // Exponer el encabezado 'X-Total-Count'
            Response.Headers.Append("Access-Control-Expose-Headers", "X-Total-Count");

            // Devolver la lista paginada de dispositivos
            return paginatedList;
        }
        [Authorize(Roles = StaticUserRoles.ADMIN + "," + StaticUserRoles.SUPERADMIN)]
        [HttpGet("{id}", Name = "GetDepartamento")]
        public async Task<ActionResult<Departamento>> GetDepartamento(int id)
        {
            var departamento = await _context.departamento.FindAsync(id);

            if (departamento == null)
            {
                return NotFound();
            }

            return departamento;
        }
        [AllowAnonymous]
        [HttpGet("all", Name = "Departamentos")]
        public async Task<ActionResult<IEnumerable<DepartamentoDTO>>> Departamentos(int id)
        {
            try
            {
                var allDepartamento = await _context.departamento.ToListAsync();
                var totalCount = allDepartamento.Count;
                var DepartamentosDTO = _mapper.Map<List<DepartamentoDTO>>(allDepartamento);

                Response.Headers["X-Total-Count"] = totalCount.ToString();
                Response.Headers.Append("Access-Control-Expose-Headers", "X-Total-Count");
                return DepartamentosDTO;
            } catch (Exception ex)
            {
                if (ex is InvalidOperationException) return StatusCode(404, "No se encontraron los datos");

                else if (ex is UnauthorizedAccessException) return StatusCode(401, "No estás autorizado para esta acción");
                
                else return StatusCode(500, $"Error al importar el archivo: {ex.Message}");
            }
        }
        [HttpGet("exportar-excel")]
        public async Task<ActionResult<DepartamentoDTO>> ExportarExcel(string filter = null)
        {
            // Obtener los datos
            var consulta = _context.departamento.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                consulta = consulta.Where(d => d.Nombre != null && d.Nombre.Contains(filter) ||
                d.Fecha_creacion.ToString() != null && d.Fecha_creacion.ToString().Contains(filter) ||
                d.Encargado != null && d.Encargado.Contains(filter));
            }
            var departamentos = await consulta.ToListAsync();
            var DepartamentosDTO = _mapper.Map<List<DepartamentoDTO>>(departamentos);

            // Crear un archivo de Excel
            using (var excel = new ExcelPackage())
            {
                var workSheet = excel.Workbook.Worksheets.Add("Departamentos");
                
                // Cargar los datos en la hoja de Excel
                workSheet.Cells.LoadFromCollection(DepartamentosDTO, true);
                // Convertir el archivo de Excel en bytes
                var excelBytes = excel.GetAsByteArray();

                string userName = User.Identity.Name;
                _auditoriaService.RegistrarAuditoria("Departamento", userName, "Reporte", "Se exportó un archivo .xlsx");
                // Devolver el archivo de Excel como un FileContentResult
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Departamentos.xlsx");
            }
        }
        [HttpPost("importar-excel")]
        public async Task<IActionResult> ImportExcel(IFormFile excel)
        {
            try
            {
                var workbook = new XLWorkbook(excel.OpenReadStream());

                var hoja = workbook.Worksheet(1);

                var primeraFilaUsada = hoja.FirstRowUsed().RangeAddress.FirstAddress.RowNumber;
                var ultimaFilaUsada = hoja.LastRowUsed().RangeAddress.FirstAddress.RowNumber;

                var departamentos = new List<Departamento>();

                for (int i = primeraFilaUsada + 1; i <= ultimaFilaUsada; i++)
                {
                    var fila = hoja.Row(i);
                    var departamentoDto = new Departamento {
                        Nombre = fila.Cell(2).GetString(),
                        Descripción = fila.Cell(3).GetString(),
                        Fecha_creacion = fila.Cell(4).GetDateTime(),
                        Encargado = fila.Cell(5).GetString()
                    };
                    
                    departamentos.Add(departamentoDto);
                }            
                _context.AddRange(departamentos);
                await _context.SaveChangesAsync();
                string userName = User.Identity.Name;
                _auditoriaService.RegistrarAuditoria("Departamento", userName, "Importar", "Se importó un archivo .xlsx");

                return Ok("Importación exitosa");
            } catch (Exception ex)
            {
                if (ex is InvalidOperationException) return StatusCode(404, "No se encontró el id");

                else if (ex is UnauthorizedAccessException) return StatusCode(401, "No estás autorizado para esta acción");
                
                else return StatusCode(500, $"Error al importar el archivo: {ex.Message}");
            }
        }
        // [Authorize(Roles = StaticUserRoles.ADMIN + "," + StaticUserRoles.SUPERADMIN)]
        [HttpPost]
        public async Task<IActionResult> saveInformation([FromBody] DepartamentoDTO departamento)
        {
                if (ModelState.IsValid)
                {
                    // Agregar el departamento al contexto y guardar los cambios en la base de datos
                    Departamento newDepartamento = _mapper.Map<Departamento>(departamento);

                    _context.departamento.Add(newDepartamento);
                    await _context.SaveChangesAsync();

                    string userName = User.Identity.Name;
                    _auditoriaService.RegistrarAuditoria("Departamentos", userName , "Agregar", "Un nuevo departamento ha sido agregado");

                    // Devolver una respuesta CreatedAtRoute con el dispositivo creado
                    return CreatedAtRoute("GetDepartamento", new { id = departamento.Id }, newDepartamento);
                }

                // Si el modelo no es válido, devolver una respuesta BadRequest
                return BadRequest(ModelState);
        }
        [Authorize(Roles = StaticUserRoles.ADMIN + "," + StaticUserRoles.SUPERADMIN)]
        [HttpPut("{id}")]
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
                string userName = User.Identity.Name;
                _auditoriaService.RegistrarAuditoria("Departamentos", userName , "Editar", "Se han editado los datos de un departamento");
                return Ok("Se actualizó correctamente");
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException) return StatusCode(404, "No se encontró el id");

                else if (ex is UnauthorizedAccessException) return StatusCode(401, "No estás autorizado para esta acción");
                return StatusCode(500, $"Ocurrió un error mientras se actualizaban los datos: {ex.Message}");
            }
        }
        [Authorize(Roles = StaticUserRoles.ADMIN + "," + StaticUserRoles.SUPERADMIN)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Departamento>> Delete(int id)
        {
            try {
                var departamento = await _context.departamento.FindAsync(id);

                if (departamento == null)
                {
                    return NotFound();
                }

                _context.departamento.Remove(departamento);
                await _context.SaveChangesAsync();
                string userName = User.Identity.Name;
                _auditoriaService.RegistrarAuditoria("Departamentos", userName , "Eliminar", "Se ha eliminado un departamento");

                return departamento;
            } catch (Exception ex)
            {
                if (ex is InvalidOperationException) return StatusCode(404, "No se encontró el id");

                else if (ex is UnauthorizedAccessException) return StatusCode(401, "No estás autorizado para esta acción");
                
                else return StatusCode(500, $"Error al importar el archivo: {ex.Message}");
            }
        }
        [AllowAnonymous]
        [HttpGet("reporte", Name = "GenerarReporteDepartamento")]
        public async Task<IActionResult> DescargarPDF(int id, string filter = null)
        {
            var consulta = _context.departamento.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                consulta = consulta.Where(d => d.Nombre != null && d.Nombre.Contains(filter) ||
                d.Fecha_creacion.ToString() != null && d.Fecha_creacion.ToString().Contains(filter) ||
                d.Encargado != null && d.Encargado.Contains(filter));
            }
            // Obtener los dispositivos paginados
            var Departamento = await consulta.ToListAsync();
            // var Departamento = await _context.departamento.ToListAsync();
            
            var data = Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(30);

                    page.Header().ShowOnce().Row(row =>
                    {
                        var rutaImagen = Path.Combine(_host.WebRootPath, "images/MIVED.png");
                        byte[] imageData = System.IO.File.ReadAllBytes(rutaImagen);

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
                            col2.Item().Text("Departamento").Underline().Bold();

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
                            
                            });

                            tabla.Header(header =>
                            {
                                header.Cell().Background("#257272").Text("Id").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("Nombre").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("Descripción").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("Fecha de creación").FontColor("#fff").FontSize(10);
                                header.Cell().Background("#257272").Text("Encargado").FontColor("#fff").FontSize(10);;
                            });

                            // var dispositivos = new List<DispositivoDTO>();
                            // var dispositivos = await _context.Dispositivos.ToListAsync();

                            // Console.WriteLine("Cantidad de dispositivos: " + dispositivos.Count);
                            foreach (var departamento in Departamento)
                            {
                                var id = departamento.Id;
                                var nombre = departamento.Nombre;
                                var descripcion = departamento.Descripción;
                                var fecha = departamento.Fecha_creacion?.ToString("dd-MM-yy");;
                                var encargado = departamento.Encargado;

                                tabla.Cell().BorderColor("#D9D9D9").Text(id).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(nombre).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(descripcion).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(fecha).FontSize(10);
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(encargado).FontSize(10);
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
            string userName = User.Identity.Name;
            _auditoriaService.RegistrarAuditoria("Departamento", userName, "Reporte", "Se exportó un archivo .pdf");
            Stream stream = new MemoryStream(data);
            return File(stream, "application/pdf", "detalledepartamento.pdf");
        }
    }
}