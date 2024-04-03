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
    [Route("api/dispositivos")]
    public class DispositivoController : Controller
    {
        private readonly ILogger<DispositivoController> _logger;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _host;
        private readonly AuditoriaService _auditoriaService;

        public DispositivoController(ILogger<DispositivoController> logger,DataContext context, IMapper mapper, IWebHostEnvironment host, AuditoriaService auditoriaService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _host = host;
            _auditoriaService = auditoriaService;        
        }
        [AllowAnonymous]
        [HttpGet(Name = "GetDispositivos")]
        public async Task<ActionResult<PaginatedList<DispositivoDTO>>> GetDispositivos(int id, int pageNumber = 1, int pageSize = 6)
        {
            var paginatedDispositivos = await (from dispositivo in _context.Dispositivos
                join departamento in _context.departamento on dispositivo.DepartamentoId equals departamento.Id
                select new DispositivoDTO
                {
                    Id = dispositivo.Id,
                    Nombre_equipo = dispositivo.Nombre_equipo,
                    Marca = dispositivo.Marca,
                    Modelo = dispositivo.Modelo,
                    Estado = dispositivo.Estado,
                    Serial_no = dispositivo.Serial_no,
                    Cod_inventario = dispositivo.Cod_inventario,
                    Bienes_nacionales = dispositivo.Bienes_nacionales,
                    Fecha_modificacion = dispositivo.Fecha_modificacion,
                    Propietario_equipo = dispositivo.Propietario_equipo,
                    Nombre_departamento = departamento.Nombre
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await _context.Dispositivos.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var paginatedList = new PaginatedList<DispositivoDTO>
            {
                TotalCount = totalCount,
                PageIndex = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                Items = paginatedDispositivos
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
                consulta = consulta.Where(d => 
                    (d.Nombre_equipo != null && d.Nombre_equipo.Contains(search)) ||
                    (d.Marca != null && d.Marca.Contains(search)) ||
                    (d.Modelo != null && d.Modelo.Contains(search)) ||
                    (d.Serial_no != null && d.Serial_no.Contains(search)) ||
                    (d.Cod_inventario != null && d.Cod_inventario.Contains(search)) ||
                    (d.Estado != null && d.Estado.Contains(search)) ||
                    (d.Propietario_equipo != null && d.Propietario_equipo.Contains(search)) ||
                    (d.Bienes_nacionales.ToString() != null && d.Bienes_nacionales.ToString().Contains(search)) ||
                    (d.departamento.Nombre != null && d.departamento.Nombre.Contains(search)));

            }

            // Realizar la consulta paginada
            var paginatedDispositivos = await consulta
                .Select(dispositivo => new DispositivoDTO
                {
                    Id = dispositivo.Id,
                    Nombre_equipo = dispositivo.Nombre_equipo,
                    Marca = dispositivo.Marca,
                    Modelo = dispositivo.Modelo,
                    Estado = dispositivo.Estado,
                    Serial_no = dispositivo.Serial_no,
                    Cod_inventario = dispositivo.Cod_inventario,
                    Bienes_nacionales = dispositivo.Bienes_nacionales,
                    Fecha_modificacion = dispositivo.Fecha_modificacion,
                    Propietario_equipo = dispositivo.Propietario_equipo,
                    Nombre_departamento = dispositivo.departamento.Nombre
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await consulta.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Mapear la lista de dispositivos a una lista de DispositivoDTO
            var dispositivosDTO = paginatedDispositivos.Select(d =>
            {
                var dispositivoDTO = _mapper.Map<DispositivoDTO>(d);
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
        [HttpGet("all/search")]
        public async Task<ActionResult<IEnumerable<DispositivoDTO>>> Search(int id, string search = null)
        {
            // Consulta inicial de dispositivos
            IQueryable<DispositivoDTO> consulta = _context.Dispositivos
                .Include(d => d.departamento)
                .Select(dispositivo => new DispositivoDTO
                {
                    Id = dispositivo.Id,
                    Nombre_equipo = dispositivo.Nombre_equipo,
                    Marca = dispositivo.Marca,
                    Modelo = dispositivo.Modelo,
                    Estado = dispositivo.Estado,
                    Serial_no = dispositivo.Serial_no,
                    Cod_inventario = dispositivo.Cod_inventario,
                    Bienes_nacionales = dispositivo.Bienes_nacionales,
                    Fecha_modificacion = dispositivo.Fecha_modificacion,
                    Propietario_equipo = dispositivo.Propietario_equipo,
                    Nombre_departamento = dispositivo.departamento.Nombre
                });

            // Filtrar por búsqueda
            if (!string.IsNullOrEmpty(search))
            {
                consulta = consulta.Where(d => 
                d.Nombre_equipo != null && d.Nombre_equipo.Contains(search) ||
                d.Modelo != null && d.Modelo.Contains(search) ||
                d.Marca != null && d.Marca.Contains(search) ||
                d.Serial_no != null && d.Serial_no.Contains(search) ||
                d.Cod_inventario != null && d.Cod_inventario.Contains(search) ||
                d.Estado != null && d.Estado.Contains(search) ||
                d.Bienes_nacionales.ToString() != null && d.Bienes_nacionales.ToString().Contains(search) ||
                d.Propietario_equipo != null && d.Propietario_equipo.Contains(search)
                );
            }

            // Realizar la consulta paginada
            var dispositivos = await consulta.ToListAsync();

            if (dispositivos == null || dispositivos.Count == 0)
            {
                return NotFound();
            }

            var totalCount = await _context.Dispositivos.CountAsync();

            // Agrega el encabezado 'X-Total-Count' a la respuesta
            Response.Headers["X-Total-Count"] = totalCount.ToString();
            // Exponer el encabezado 'X-Total-Count'
            Response.Headers.Append("Access-Control-Expose-Headers", "X-Total-Count");

            return dispositivos;
        }
        [Authorize(Roles = StaticUserRoles.SOPORTE+ "," + StaticUserRoles.ADMIN + "," + StaticUserRoles.SUPERADMIN)]
        [HttpGet("{id}", Name = "GetDispositivo")]
        public async Task<ActionResult<DispositivoDTO>> GetDispositivo(int id)
        {
            var dispositivo = await _context.Dispositivos
                .Include(d => d.departamento)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dispositivo == null)
            {
                return NotFound();
            }

            // Mapear el dispositivo a un DTO que incluya el nombre del departamento
            var dispositivoDTO = new DispositivoDTO
            {
                Id = dispositivo.Id,
                Nombre_equipo = dispositivo.Nombre_equipo,
                Marca = dispositivo.Marca,
                Modelo = dispositivo.Modelo,
                Estado = dispositivo.Estado,
                Serial_no = dispositivo.Serial_no,
                Cod_inventario = dispositivo.Cod_inventario,
                Bienes_nacionales = dispositivo.Bienes_nacionales,
                Fecha_modificacion = dispositivo.Fecha_modificacion,
                Propietario_equipo = dispositivo.Propietario_equipo,
                Nombre_departamento = dispositivo.departamento != null ? dispositivo.departamento.Nombre : null
            };

            return dispositivoDTO;
        }
        [Authorize(Roles = StaticUserRoles.SOPORTE+ "," + StaticUserRoles.ADMIN + "," + StaticUserRoles.SUPERADMIN)]
        [HttpGet("all", Name = "Dispositivos")]
        public async Task<ActionResult<IEnumerable<DispositivoDTO>>> Dispositivos()
        {
            IQueryable<DispositivoDTO> consulta = _context.Dispositivos
                .Include(d => d.departamento)
                .Select(dispositivo => new DispositivoDTO
                {
                    Id = dispositivo.Id,
                    Nombre_equipo = dispositivo.Nombre_equipo,
                    Marca = dispositivo.Marca,
                    Modelo = dispositivo.Modelo,
                    Estado = dispositivo.Estado,
                    Serial_no = dispositivo.Serial_no,
                    Cod_inventario = dispositivo.Cod_inventario,
                    Bienes_nacionales = dispositivo.Bienes_nacionales,
                    Fecha_modificacion = dispositivo.Fecha_modificacion,
                    Propietario_equipo = dispositivo.Propietario_equipo,
                    Nombre_departamento = dispositivo.departamento.Nombre
                });

            var dispositivos = await consulta.ToListAsync();

            if (dispositivos == null || dispositivos.Count == 0)
            {
                return NotFound();
            }

            var totalCount = await _context.Dispositivos.CountAsync();
            
            Response.Headers["X-Total-Count"] = totalCount.ToString();
            Response.Headers.Append("Access-Control-Expose-Headers", "X-Total-Count");

            return dispositivos;
        }
        [HttpGet("exportar-excel"), AllowAnonymous]
        public async Task<IActionResult> ExportarExcel(string filter = null)
        {
            // Obtener los datos
            IQueryable<Dispositivo> consulta = _context.Dispositivos.Include(d => d.departamento);

            // Filtrar por búsqueda si se proporciona
            if (!string.IsNullOrEmpty(filter))
            {
                consulta = consulta.Where(d => 
                d.Nombre_equipo != null && d.Nombre_equipo.Contains(filter) ||
                d.Serial_no != null && d.Serial_no.Contains(filter) ||
                d.Cod_inventario != null && d.Cod_inventario.Contains(filter) ||
                d.Estado != null && d.Estado.Contains(filter) ||
                d.Bienes_nacionales.ToString() != null && d.Bienes_nacionales.ToString().Contains(filter) ||
                d.departamento.Nombre != null && d.departamento.Nombre.Contains(filter));
            }
            var dispositivos = 
            await consulta
            // await _context.Dispositivos
                // .Include(d => d.departamento)
                .Select(dispositivo => new
                {
                    dispositivo.Id,
                    dispositivo.Nombre_equipo,
                    dispositivo.Marca,
                    dispositivo.Modelo,
                    dispositivo.Estado,
                    dispositivo.Serial_no,
                    dispositivo.Cod_inventario,
                    dispositivo.Bienes_nacionales,
                    dispositivo.Fecha_modificacion,
                    dispositivo.Propietario_equipo,
                    Nombre_departamento = dispositivo.departamento.Nombre
                })
                .ToListAsync();

            // Crear un archivo de Excel
            using (var excel = new ExcelPackage())
            {
                var workSheet = excel.Workbook.Worksheets.Add("Equipos");
                
                // Cargar los datos en la hoja de Excel
                workSheet.Cells.LoadFromCollection(dispositivos, true);

                // Convertir el archivo de Excel en bytes
                var excelBytes = excel.GetAsByteArray();

                string userName = User.Identity.Name;
                _auditoriaService.RegistrarAuditoria("Dispositivos", userName, "Reporte", "Se exportó un archivo .xlsx");

                // Devolver el archivo de Excel como un FileContentResult
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Equipos.xlsx");
            }
        }
        // [Authorize(Roles = StaticUserRoles.SOPORTE+ "," + StaticUserRoles.ADMIN + "," + StaticUserRoles.SUPERADMIN)]
        [HttpPost]
        public async Task<IActionResult> saveInformation([FromBody] DispositivoCreateDTO dispositivo)
        {
            if (ModelState.IsValid)
            {
                Dispositivo newDispositivo = _mapper.Map<Dispositivo>(dispositivo);
                newDispositivo.DepartamentoId = dispositivo.DepartamentoId;
                _context.Dispositivos.AddAsync(newDispositivo);
                await _context.SaveChangesAsync();

                string userName = User.Identity.Name;

                _auditoriaService.RegistrarAuditoria("Dispositivos", userName , "Agregar", "Se ha agregado un nuevo registro");
                // Devolver una respuesta CreatedAtRoute con el dispositivo creado
                return CreatedAtRoute("Getdispositivo", new { id = newDispositivo.Id }, newDispositivo);
            }

            return BadRequest(ModelState);
        }
        [Authorize(Roles = StaticUserRoles.SOPORTE+ "," + StaticUserRoles.ADMIN + "," + StaticUserRoles.SUPERADMIN)]
        [HttpPost("importar-excel")]
        public async Task<IActionResult> ImportExcel(IFormFile excel)
        {
            try {

                if (excel == null || excel.Length == 0)
                {
                    return BadRequest("No se ha seleccionado ningún archivo o el archivo está vacío.");
                }
                var workbook = new XLWorkbook(excel.OpenReadStream());

                var hoja = workbook.Worksheet(1);

                var primeraFilaUsada = hoja.FirstRowUsed().RangeAddress.FirstAddress.RowNumber;
                var ultimaFilaUsada = hoja.LastRowUsed().RangeAddress.FirstAddress.RowNumber;

                var dispositivos = new List<Dispositivo>();

                for (int i = primeraFilaUsada + 1; i <= ultimaFilaUsada; i++)
                {
                    var fila = hoja.Row(i);
                    var dispositivoDTO = new DispositivoDTO {
                        Nombre_equipo = fila.Cell(2).GetString(),
                        Marca = fila.Cell(3).GetString(),
                        Modelo = fila.Cell(4).GetString(),
                        Estado = fila.Cell(5).GetString(),
                        Serial_no = fila.Cell(6).GetString(),
                        Cod_inventario = fila.Cell(7).GetString(),
                        Bienes_nacionales = fila.Cell(8).GetValue<int>(),
                        Fecha_modificacion = fila.Cell(9).GetDateTime(),
                        Propietario_equipo = fila.Cell(10).GetString(),
                        Nombre_departamento = fila.Cell(11).GetString()
                    };

                    var departamento = await _context.departamento.FirstOrDefaultAsync(d => d.Nombre == dispositivoDTO.Nombre_departamento);
                    if (departamento != null)
                    {
                        var dispositivo = new Dispositivo {
                            Nombre_equipo = dispositivoDTO.Nombre_equipo,
                            Marca = dispositivoDTO.Marca,
                            Modelo = dispositivoDTO.Modelo,
                            Estado = dispositivoDTO.Estado,
                            Serial_no = dispositivoDTO.Serial_no,
                            Cod_inventario = dispositivoDTO.Cod_inventario,
                            Bienes_nacionales = dispositivoDTO.Bienes_nacionales,
                            Fecha_modificacion = dispositivoDTO.Fecha_modificacion,
                            Propietario_equipo = dispositivoDTO.Propietario_equipo,
                            DepartamentoId = departamento.Id
                        };
                        dispositivos.Add(dispositivo);
                    }
                }
                _context.AddRange(dispositivos);
                await _context.SaveChangesAsync();
                string userName = User.Identity.Name;
                _auditoriaService.RegistrarAuditoria("Dispositivos", userName, "Importar", "Se importó un archivo .xlsx");

                return Ok("Importación exitosa");
            } catch (Exception ex)
            {
                if (ex is InvalidOperationException) return StatusCode(404, "No se encontró lo que solicitó");

                else if (ex is UnauthorizedAccessException) return StatusCode(401, "No estás autorizado para esta acción");
                
                else return StatusCode(500, $"Error al importar el archivo: {ex.Message}");
            }
        }
        [Authorize(Roles = StaticUserRoles.SOPORTE+ "," + StaticUserRoles.ADMIN + "," + StaticUserRoles.SUPERADMIN)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] DispositivoCreateDTO dispositivo)
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
                newDispositivo.DepartamentoId = dispositivo.DepartamentoId;
                _context.Update(newDispositivo);
                await _context.SaveChangesAsync();
                string userName = User.Identity.Name;

                _auditoriaService.RegistrarAuditoria("Dispositivos", userName , "Editar", "Se ha editado un dispositivo");
                return Ok("Se actualizó correctamente");
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException) return StatusCode(404, "No se encontró lo que solicitó");

                else if (ex is UnauthorizedAccessException) return StatusCode(401, "No estás autorizado para esta acción");

                else {
                return StatusCode(500, $"Ocurrió un error mientras se actualizaban los datos: {ex.Message}");
                }
            }
        }
        [Authorize(Roles = StaticUserRoles.SOPORTE+ "," + StaticUserRoles.ADMIN + "," + StaticUserRoles.SUPERADMIN)]
        [HttpDelete("{id}")] 
        public async Task<ActionResult<Dispositivo>> Delete(int id)
        {
            try{
                var dispositivo = await _context.Dispositivos.FindAsync(id);
                if (dispositivo == null)
                {
                    return NotFound();
                }

                _context.Dispositivos.Remove(dispositivo);
                await _context.SaveChangesAsync();
                string userName = User.Identity.Name;
                _auditoriaService.RegistrarAuditoria("Dispositivos", userName , "Eliminar", "Se ha eliminado un dispositivo");

                return dispositivo;
            } catch (Exception ex) {
                if (ex is UnauthorizedAccessException)
                {
                    return StatusCode(401, "No estás autorizado para esta acción");
                }
                else {
                return StatusCode(500, $"Ocurrió un error mientras se actualizaban los datos: {ex.Message}");
                }
            }
        }
        [AllowAnonymous]
        [HttpGet("reporte", Name = "GenerarReporteDispositivos")]
        public async Task<IActionResult> DescargarPDF(int id, string filter = null)
        {
            IQueryable<Dispositivo> consulta = _context.Dispositivos.Include(d => d.departamento);

            // Filtrar por búsqueda si se proporciona
            if (!string.IsNullOrEmpty(filter))
            {
                consulta = consulta.Where(d => d.Estado != null && d.Estado.Contains(filter));
            }

            // var dispositivos = await (from dispositivo in _context.Dispositivos
            //     join departamento in _context.departamento on dispositivo.DepartamentoId equals departamento.Id
            //     select new { Dispositivo = dispositivo, Departamento = departamento })
            var dispositivos= await consulta
                .Select(d => new
                {
                    Dispositivo = d,
                    Nombre_departamento = d.departamento.Nombre
                })
                .ToListAsync();
            
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
                                header.Cell().Background("#257272").Text("Departamento").FontColor("#fff").FontSize(10);
                            });

                            foreach (var dispositivo in dispositivos)
                            {
                                var id = dispositivo.Dispositivo.Id;
                                var nombre = dispositivo.Dispositivo.Nombre_equipo;
                                var marca = dispositivo.Dispositivo.Marca;
                                var modelo = dispositivo.Dispositivo.Modelo;
                                var estado = dispositivo.Dispositivo.Estado;
                                var noSerial = dispositivo.Dispositivo.Serial_no;
                                var invi = dispositivo.Dispositivo.Cod_inventario;
                                var bienes = dispositivo.Dispositivo.Bienes_nacionales.ToString();
                                var fecha = dispositivo.Dispositivo.Fecha_modificacion?.ToString("dd-MM-yy");
                                var propietario = dispositivo.Dispositivo.Propietario_equipo;
                                var Nombre_departamento = dispositivo.Dispositivo.departamento.Nombre;

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
                                tabla.Cell().BorderColor("#D9D9D9").Padding(2).Text(Nombre_departamento).FontSize(10);
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
            _auditoriaService.RegistrarAuditoria("Dispositivos", userName, "Reporte", "Se exportó un archivo .pdf");
            Stream stream = new MemoryStream(data);
            return File(stream, "application/pdf", "detalledispositivos.pdf");
        }
    }
}