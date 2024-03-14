using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Inventario.Authorization;
using Inventario.DTOs;
using Inventario.Interface;
using Inventario.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Inventario.Controllers
{
    [ApiController]
    [Route("api/")]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;

        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }

        // Route For Seeding my roles to DB
        [HttpPost]
        [Route("seed-roles")]
        public async Task<IActionResult> SeedRoles()
        {
            var seerRoles = await _authService.SeedRolesAsync();
            return Ok(seerRoles);
        }

        // Route -> Register
        [HttpPost]
        [Route("registro")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDto)
        {
            var registerResult = await _authService.RegisterAsync(userDto);

            if (registerResult.IsSucceed) 
                return Ok(registerResult);

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

        // Route -> make user -> soporte
        [HttpPost]
        [Route("add-Soporte")]
        public async Task<IActionResult> AddSoporte([FromBody] UpdatePermissionDto updatePermissionDto)
        {
            var operationResult = await _authService.AddSoporteAsync(updatePermissionDto);

            if (operationResult.IsSucceed) return Ok(operationResult);

            return BadRequest(operationResult);
        }
    }
}