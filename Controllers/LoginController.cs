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
using Inventario.Interface;
using Inventario.Authorization;
using AutoMapper;

namespace Inventario.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<LoginController> _logger;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        // private readonly IWebHostEnvironment _host;
        private readonly AuditoriaService _auditoriaService;

        public LoginController(IAuthService authService, ILogger<LoginController> logger, DataContext context, IMapper mapper, AuditoriaService auditoriaService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _logger = logger;
            _context = context;
            _mapper = mapper;
            // _host = host;
            _auditoriaService = auditoriaService;
        }

        // Route For Seeding my roles to DB
        [HttpPost]
        [Route("seed-roles")]
        public async Task<ActionResult<User>> SeedRoles([FromBody] UpdatePermissionDto updatePermissionDto)
        {
            var seerRoles = await _authService.SeedRolesAsync();
            
            UserCreateDTO usuario = new UserCreateDTO {FirstName = "SuperAdmin", LastName = "SuperAdmin", 
            UserName = "superadmin", Email = "no tiene", Password = "Fei234Bmcd_12"};
            
            UpdatePermissionDto Rol = new UpdatePermissionDto {UserName = "superadmin"};

            var registerResult = await _authService.RegisterAsync(usuario);
            var operationResult = await _authService.AddSuperAdminAsync(Rol);
            // if(operationResult.IsSucceed) return Ok(operationResult);

            if (registerResult.IsSucceed) 
            {
            User newUser = _mapper.Map<User>(usuario);

            _context.usuarios.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return Ok(seerRoles);
            return Ok(operationResult);
            }
            else
            {
                // return BadRequest(registerResult.Message);
                return BadRequest(registerResult);
            }
        }
        [Authorize(Roles = StaticUserRoles.ADMIN + "," + StaticUserRoles.SUPERADMIN)]
        [HttpGet(Name = "GetUsuarios")]
        public async Task<ActionResult<PaginatedList<UserDTO>>> GetUsuarios(int pageNumber = 1, int pageSize = 6)
        {
            var paginatedUsers = await _context.usuarios
                .Select(user => new UserDTO
                {
                    // Mapea los campos de usuario a los campos correspondientes en UserDTO
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await _context.usuarios.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var paginatedList = new PaginatedList<UserDTO>
            {
                TotalCount = totalCount,
                PageIndex = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                Items = paginatedUsers
            };

            // Agrega el encabezado 'X-Total-Count' a la respuesta
            Response.Headers["X-Total-Count"] = totalCount.ToString();
            // Exponer el encabezado 'X-Total-Count'
            Response.Headers.Append("Access-Control-Expose-Headers", "X-Total-Count");

            return paginatedList;
        }
        [HttpGet("search"), AllowAnonymous]
        public async Task<ActionResult<PaginatedList<UserDTO>>> Search(int pageNumber = 1, int pageSize = 6, string search = null)
        {

            IQueryable<User> consulta = _context.usuarios;
            // Filtrar por búsqueda si se proporciona
            if (!string.IsNullOrEmpty(search))
            {
                consulta = consulta.Where(d => d.FirstName.Contains(search));
            }

            var paginatedUsers = await _context.usuarios
                .Select(user => new UserDTO
                {
                    // Mapea los campos de usuario a los campos correspondientes en UserDTO
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await _context.usuarios.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var paginatedList = new PaginatedList<UserDTO>
            {
                TotalCount = totalCount,
                PageIndex = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                Items = paginatedUsers
            };

            // Agrega el encabezado 'X-Total-Count' a la respuesta
            Response.Headers["X-Total-Count"] = totalCount.ToString();
            // Exponer el encabezado 'X-Total-Count'
            Response.Headers.Append("Access-Control-Expose-Headers", "X-Total-Count");

            return paginatedList;
        }
        [HttpGet("{id}", Name = "GetUsario")]
        // [Route("usuario")]
        public async Task<ActionResult<User>> GetData(int id)
        {
            var user = await _context.usuarios.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
        [Authorize(Roles = StaticUserRoles.ADMIN + "," + StaticUserRoles.SUPERADMIN)]
        [HttpPost]
        [Route("registro")]
        public async Task<ActionResult<User>> Register([FromBody] UserCreateDTO userDto)
        {
            var registerResult = await _authService.RegisterAsync(userDto);

            if (registerResult.IsSucceed) 
            {
                User newUser = _mapper.Map<User>(userDto);

                _context.usuarios.AddAsync(newUser);
                await _context.SaveChangesAsync();

                return CreatedAtRoute("GetUsario", new { id = newUser.Id }, newUser);
            }

            return BadRequest(registerResult);
        }
        // Route -> Login
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var loginResult = await _authService.LoginAsync(loginDto);

            if(loginResult.IsSucceed) return Ok(loginResult);

            return Unauthorized(loginResult);
        }
        // Route -> make user -> admin
        // [Authorize(Roles = StaticUserRoles.SUPERADMIN)]
        [HttpPost]
        [Route("add-Admin")]
        public async Task<IActionResult> AddAdmin([FromBody] UpdatePermissionDto updatePermissionDto)
        {
            var operationResult = await _authService.AddAdminAsync(updatePermissionDto);

            if(operationResult.IsSucceed) return Ok(operationResult);

            return BadRequest(operationResult);
        }
        // Route -> make user -> soporte
        [Authorize(Roles = StaticUserRoles.ADMIN + "," + StaticUserRoles.SUPERADMIN)]
        [HttpPost]
        [Route("add-Soporte")]
        public async Task<IActionResult> AddSoporte([FromBody] UpdatePermissionDto updatePermissionDto)
        {
            var operationResult = await _authService.AddSoporteAsync(updatePermissionDto);

            if (operationResult.IsSucceed) return Ok(operationResult);

            return BadRequest(operationResult);
        }
        [Authorize(Roles = StaticUserRoles.SUPERADMIN)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserCreateDTO user)
        {
            if (id != user?.Id)
            {
                return BadRequest("No se encontró el ID");
            }

            else if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                User newUser = _mapper.Map<User>(user);
                _context.Update(newUser);
                await _context.SaveChangesAsync();
                return Ok("Se actualizó correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error mientras se actualizaban los datos: {ex.Message}");
            }
        }
        // [Authorize(Roles = StaticUserRoles.SUPERADMIN)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> Delete(string id)
        {
            var user = await _context.usuarios.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.usuarios.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }
}