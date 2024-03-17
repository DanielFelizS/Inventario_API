using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Inventario.Authorization;
using Inventario.DTOs;
using Inventario.Interface;
using Inventario.Services;
using Inventario.Models;
using Inventario.Authorization;
using Inventario.Data;
using Inventario.Controllers;
using Inventario.AutoMapperConfig;
using AutoMapper;

namespace Inventario.Controllers
{
    [ApiController]
    [Route("api/")]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<LoginController> _logger;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        // private readonly IWebHostEnvironment _host;

        public LoginController(IAuthService authService, ILogger<LoginController> logger, DataContext context, IMapper mapper)
        {
            _authService = authService;
            _logger = logger;
            _context = context;
            _mapper = mapper;
            // _host = host;
        }

        // Route For Seeding my roles to DB
        [HttpPost]
        [Route("seed-roles")]
        public async Task<IActionResult> SeedRoles()
        {
            var seerRoles = await _authService.SeedRolesAsync();
            return Ok(seerRoles);
        }
        [HttpGet("{id}", Name = "GetUsarios")]
        public async Task<ActionResult<User>> GetData(int id)
        {
            var user = await _context.usuarios.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        [Route("registro")]
        public async Task<ActionResult<User>> Register([FromBody] UserDTO userDto)
        {
            var registerResult = await _authService.RegisterAsync(userDto);

            if (registerResult.IsSucceed) 
            {
                User newUser = _mapper.Map<User>(userDto);
                _context.usuarios.AddAsync(newUser);
                await _context.SaveChangesAsync();

                return CreatedAtRoute("GetUsarios", new { id = newUser.Id }, newUser);
            }

            return BadRequest(registerResult);
        }

        // Route -> Login
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var loginResult = await _authService.LoginAsync(loginDto);

            if(loginResult.IsSucceed) return Ok(loginResult);

            return Unauthorized(loginResult);
        }

        // Route -> make user -> admin
        [HttpPost]
        [Route("add-Admin")]
        public async Task<IActionResult> AddAdmin([FromBody] UpdatePermissionDto updatePermissionDto)
        {
            var operationResult = await _authService.AddAdminAsync(updatePermissionDto);

            if(operationResult.IsSucceed) return Ok(operationResult);

            return BadRequest(operationResult);
        }
        // Route -> remove user -> admin
        [HttpDelete]
        [Route("delete-Admin")]
        public async Task<IActionResult> RemoveAdmin([FromBody] UpdatePermissionDto updatePermissionDto)
        {
            var operationResult = await _authService.RemoveAdminAsync(updatePermissionDto);

            if(operationResult.IsSucceed) return Ok(operationResult);

            return BadRequest(operationResult);
        }

        // Route -> make user -> soporte
        [HttpPost]
        [Route("add-Soporte")]
        public async Task<IActionResult> AddSoporte([FromBody] UpdatePermissionDto updatePermissionDto)
        {
            var operationResult = await _authService.AddSoporteAsync(updatePermissionDto);

            if (operationResult.IsSucceed) return Ok(operationResult);

            return BadRequest(operationResult);
        }

        // Route -> remove user -> soporte
        [HttpDelete]
        [Route("delete-Soporte")]
        public async Task<IActionResult> RemoveSoporte([FromBody] UpdatePermissionDto updatePermissionDto)
        {
            var operationResult = await _authService.RemoveSoporteAsync(updatePermissionDto);

            if (operationResult.IsSucceed) return Ok(operationResult);

            return BadRequest(operationResult);
        }
    }
}