using Inventario.DTOs;
using Inventario.Interface;
using Inventario.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


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
                    Message = "Usuario o contraseña incorrecto"
                };

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!isPasswordCorrect)
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Usuario o contraseña incorrecto"
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
        public async Task<AuthServiceResponseDto> AddSuperAdminAsync(UpdatePermissionDto updatePermissionDto)
        {
            var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

            if (user is null)
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Nombre de usuario incorrecto"
                };

            // Verificar si el rol existe antes de intentar agregarlo
            if (!await _roleManager.RoleExistsAsync(StaticUserRoles.SUPERADMIN))
            {
                // El rol no existe, manejar el error apropiadamente
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "El rol 'SUPERADMIN' no existe"
                };
            }

            // El rol existe, agregar el usuario al rol
            await _userManager.AddToRoleAsync(user, StaticUserRoles.SUPERADMIN);
            await _userManager.RemoveFromRoleAsync(user, StaticUserRoles.ADMIN);
            await _userManager.RemoveFromRoleAsync(user, StaticUserRoles.Lector);
            await _userManager.RemoveFromRoleAsync(user, StaticUserRoles.SOPORTE);

            // User Superadmin = new User {UserName = "SuperAdmin"};

            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = $"El usuario {updatePermissionDto.UserName} es ahora un superadmin"
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

            // User Superadmin = new User {UserName = "SuperAdmin"};

            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = $"El usuario {updatePermissionDto.UserName} es ahora un admin"
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
                Message = $"El usuario {updatePermissionDto.UserName} es ahora parte del equipo de soporte técnico"
            };
        }
        public async Task<AuthServiceResponseDto> RegisterAsync(UserCreateDTO userDto)
        {
            var isExistsUser = await _userManager.FindByNameAsync(userDto.UserName);
            var isExistsEmail = await _userManager.FindByNameAsync(userDto.Email);

            if (isExistsUser != null)
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = $"El usuario {userDto.UserName} ya existe"
                };

            else if (isExistsEmail != null)
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = $"El correo electrónico {userDto.Email} ya existe"
                };
            
            ApplicationUser newUser = new ApplicationUser()
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                UserName = userDto.UserName,
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

            // Rol por defecto al registrarse un usuario
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
            bool isSuperAdminRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.SUPERADMIN);
            bool isLectorRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.Lector);

            if (isSoporteRoleExists && isAdminRoleExists && isLectorRoleExists && isSuperAdminRoleExists)
                return new AuthServiceResponseDto()
                {
                    IsSucceed = true,
                    Message = "Roles Seeding is Already Done"
                };

            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.Lector));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.SUPERADMIN));
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
                    expires: DateTime.Now.AddHours(8),
                    claims: claims,
                    signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
                );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

            return token;
        }
    }
}