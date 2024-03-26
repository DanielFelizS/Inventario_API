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

namespace Inventario.Controllers
{
    [Route("api/historial")]
    public class HistorialController : Controller
    {
        private readonly ILogger<HistorialController> _logger;
        private readonly DataContext _context;

        public HistorialController(ILogger<HistorialController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;

        }
        [HttpGet("all"), AllowAnonymous]
        public async Task<ActionResult<PaginatedList<Auditoria>>> VerHistorial(int id, int pageNumber = 1, int pageSize = 6)
        {
            var datos = await _context.historial.FindAsync(id);
            var Historial = await _context.historial.ToListAsync();
            var totalCount = Historial.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            // var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var paginatedHistorial = Historial.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedList = new PaginatedList<Auditoria>
            {
                Items = paginatedHistorial,
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
        public async Task<ActionResult<PaginatedList<Auditoria>>> Search(int id, int pageNumber = 1, int pageSize = 6, string search = null)
        {
            // Consulta inicial de dispositivos
            IQueryable<Auditoria> consulta = _context.historial;

            // Filtrar por búsqueda si se proporciona
            if (!string.IsNullOrEmpty(search))
            {
                consulta = consulta.Where(d => 
                d.Tabla != null && d.Tabla.Contains(search) ||
                d.Usuario != null && d.Usuario.Contains(search) ||
                d.Acción != null && d.Acción.Contains(search) ||
                d.Descripción != null && d.Descripción.Contains(search));
            }

            // Realizar la consulta paginada
            var paginatedHistorial = await consulta
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await consulta.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Mapear la lista de dispositivos a una lista de DispositivoDTO
            // var Historial = paginatedHistorial.Select(d =>
            // {
            //     return dispositivoDTO;
            // }).ToList();

            var paginatedList = new PaginatedList<Auditoria>
            {
                Items = paginatedHistorial,
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
        [HttpGet("exportar-excel"), AllowAnonymous]
        public async Task<IActionResult> ExportarExcel(string filter = null)
        {
            // Obtener los datos
            IQueryable<Auditoria> consulta = _context.historial;

            // Filtrar por búsqueda si se proporciona
            if (!string.IsNullOrEmpty(filter))
            {
                consulta = consulta.Where(d => 
                d.Tabla != null && d.Tabla.Contains(filter) ||
                d.Usuario != null && d.Usuario.Contains(filter) ||
                d.Acción != null && d.Acción.Contains(filter) ||
                d.Descripción != null && d.Descripción.Contains(filter));
            }
            var Historial = await consulta.ToListAsync();
            // Crear un archivo de Excel
            using (var excel = new ExcelPackage())
            {
                var workSheet = excel.Workbook.Worksheets.Add("Auditorias");
                
                // Cargar los datos en la hoja de Excel
                workSheet.Cells.LoadFromCollection(Historial, true);

                // Convertir el archivo de Excel en bytes
                var excelBytes = excel.GetAsByteArray();

                // Devolver el archivo de Excel como un FileContentResult
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Auditorias.xlsx");
            }
        }
    }
}