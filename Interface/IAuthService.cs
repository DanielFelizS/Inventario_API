using Inventario.DTOs;

namespace Inventario.Interface
{
    public interface IAuthService
    {
        Task<AuthServiceResponseDto> SeedRolesAsync();
        Task<AuthServiceResponseDto> RegisterAsync(UserDTO userDto);
        Task<AuthServiceResponseDto> LoginAsync(LoginDTO loginDto);
        Task<AuthServiceResponseDto> AddAdminAsync(UpdatePermissionDto updatePermissionDto);
        Task<AuthServiceResponseDto> AddSoporteAsync(UpdatePermissionDto updatePermissionDto);
    }
}