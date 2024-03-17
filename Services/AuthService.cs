using Inventario.DTOs;
using Inventario.Interface;
using Inventario.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;


namespace Inventario.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<AuthServiceResponseDto> LoginAsync(LoginDTO loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user is null)
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Credenciales inválidas"
                };

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!isPasswordCorrect)
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Credenciales inválidas"
                };

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("JWTID", Guid.NewGuid().ToString()),
                new Claim("Nombre", user.FirstName),
                new Claim("Apellido", user.LastName),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GenerateNewJsonWebToken(authClaims);

            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = token
            };
        }

        public async Task<AuthServiceResponseDto> AddAdminAsync(UpdatePermissionDto updatePermissionDto)
        {
            var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

            if (user is null)
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Nombre de usuario incorrecto"
                };

            // Verificar si el rol existe antes de intentar agregarlo
            if (!await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN))
            {
                // El rol no existe, manejar el error apropiadamente
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "El rol 'ADMIN' no existe"
                };
            }

            // El rol existe, agregar el usuario al rol
            await _userManager.AddToRoleAsync(user, StaticUserRoles.ADMIN);
            await _userManager.RemoveFromRoleAsync(user, StaticUserRoles.Lector);
            await _userManager.RemoveFromRoleAsync(user, StaticUserRoles.SOPORTE);


            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = "El usuario es ahora un admin"
            };
        }

        public async Task<AuthServiceResponseDto> RemoveAdminAsync(UpdatePermissionDto updatePermissionDto)
        {
            var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

            if (user is null)
            {
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Nombre de usuario incorrecto"
                };
            }

            // Verificar si el usuario tiene el rol de ADMIN antes de intentar eliminarlo
            if (!await _userManager.IsInRoleAsync(user, StaticUserRoles.ADMIN))
            {
                // El usuario no tiene el rol ADMIN, manejar el error apropiadamente
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "El usuario no es un admin"
                };
            }

            // El usuario tiene el rol ADMIN, eliminar el rol del usuario
            await _userManager.RemoveFromRoleAsync(user, StaticUserRoles.ADMIN);
            await _userManager.AddToRoleAsync(user, StaticUserRoles.Lector);

            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = "El usuario ya no es un admin"
            };
        }

        public async Task<AuthServiceResponseDto> AddSoporteAsync(UpdatePermissionDto updatePermissionDto)
        {
            var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

            if (user is null)
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Nombre de usuario incorrecto"
                };

            await _userManager.AddToRoleAsync(user, StaticUserRoles.SOPORTE);
            await _userManager.RemoveFromRoleAsync(user, StaticUserRoles.Lector);
            await _userManager.RemoveFromRoleAsync(user, StaticUserRoles.ADMIN);

            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = "El usuario es ahora parte del equipo de soporte técnico"
            };
        }

        public async Task<AuthServiceResponseDto> RemoveSoporteAsync(UpdatePermissionDto updatePermissionDto)
        {
            var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

            if (user is null)
            {
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Nombre de usuario incorrecto"
                };
            }

            // Verificar si el usuario tiene el rol de SOPORTE antes de intentar eliminarlo
            if (!await _userManager.IsInRoleAsync(user, StaticUserRoles.SOPORTE))
            {
                // El usuario no tiene el rol SOPORTE, manejar el error apropiadamente
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "El usuario no es parte del equipo de soporte técnico"
                };
            }

            // El usuario tiene el rol SOPORTE, eliminar el rol del usuario
            await _userManager.RemoveFromRoleAsync(user, StaticUserRoles.SOPORTE);
            await _userManager.AddToRoleAsync(user, StaticUserRoles.Lector);

            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = "El usuario ya no es parte del equipo de soporte técnico"
            };
        }

        public async Task<AuthServiceResponseDto> RegisterAsync(UserDTO userDto)
        {
            var isExistsUser = await _userManager.FindByNameAsync(userDto.UserName);

            if (isExistsUser != null)
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "El usuario ya existe"
                };
            
            ApplicationUser newUser = new ApplicationUser()
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                UserName = userDto.UserName,
                // UserRol = userDto.UserRol,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var createUserResult = await _userManager.CreateAsync(newUser, userDto.Password);

            if (!createUserResult.Succeeded)
            {
                var errorString = "El usuario no se ha agregado porque: ";
                foreach (var error in createUserResult.Errors)
                {
                    errorString += " # " + error.Description;
                }
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = errorString
                };
            }

            // Add a Default Lector Role to all users
            await _userManager.AddToRoleAsync(newUser, StaticUserRoles.Lector);

            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = "El usuario ha sido creado correctamente"
            };
        }

        public async Task<AuthServiceResponseDto> SeedRolesAsync()
        {
            bool isSoporteRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.SOPORTE);
            bool isAdminRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);
            bool isLectorRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.Lector);

            if (isSoporteRoleExists && isAdminRoleExists && isLectorRoleExists)
                return new AuthServiceResponseDto()
                {
                    IsSucceed = true,
                    Message = "Roles Seeding is Already Done"
                };

            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.Lector));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.SOPORTE));
        
            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = "Role Seeding Done Successfully"
            };
        }

        private string GenerateNewJsonWebToken(List<Claim> claims)
        {
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));

            var tokenObject = new JwtSecurityToken(
                    issuer: _configuration["JwtSettings:ValidIssuer"],
                    audience: _configuration["JwtSettings:ValidAudience"],
                    expires: DateTime.Now.AddHours(1),
                    claims: claims,
                    signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
                );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

            return token;
        }
    }
}