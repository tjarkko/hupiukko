using Hupiukko.Api.Dtos;

namespace Hupiukko.Api.BusinessLogic.Managers;

public interface IUsersManager
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<UserDto> CreateUserAsync(CreateUserRequest request);
    Task<UserDto?> UpdateUserAsync(Guid id, CreateUserRequest request);
    Task<bool> DeleteUserAsync(Guid id);
} 