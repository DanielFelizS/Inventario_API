using Inventario.DTOs;

namespace Inventario.Interface
{
    public interface IAuthService
    {
        
        Task<AuthServiceResponseDto> SeedRolesAsync();
        Task<AuthServiceResponseDto> RegisterAsync(UserCreateDTO userDto);
        Task<AuthServiceResponseDto> LoginAsync(LoginDTO loginDto);
        Task<AuthServiceResponseDto> AddSuperAdminAsync(UpdatePermissionDto updatePermissionDto);
        Task<AuthServiceResponseDto> AddAdminAsync(UpdatePermissionDto updatePermissionDto);
        Task<AuthServiceResponseDto> AddSoporteAsync(UpdatePermissionDto updatePermissionDto);
    }
}