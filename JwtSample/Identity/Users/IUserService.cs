using JwtSample.Dto;
using JwtSample.Interfaces;

namespace JwtSample.Identity.Users;

public interface IUserService : ITransientService
{
    Task<UserDetailDto> GetAsync(string userName);

    Task<List<UserDetailDto>> GetAsync();

    Task CreateAsync(UserDto request);

    Task<string> UpdateAsync(UserDto request, string username);

    Task<string> DeleteAsync(string userName);

    Task<string> UnlockAsync(string userName);
}