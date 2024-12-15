using JwtSample.Data;
using JwtSample.Dto;
using JwtSample.Entities;
using JwtSample.Exceptions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using static JwtSample.Common.MOneCrypto;

namespace JwtSample.Identity.Users;

internal sealed class UserService : IUserService
{
    private readonly DataContext _dataContext;

    public UserService(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task CreateAsync(UserDto request)
    {
        if (await _dataContext.Users.FindAsync(request.UserName) is not null)
        {
            throw new UserAlreadyExistsException(request.UserName);
        }

        CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new User(request.UserName, passwordHash, passwordSalt, true);

        _dataContext.Users.Add(user);
        await _dataContext.SaveChangesAsync();
    }

    public async Task<string> DeleteAsync(string userName)
    {
        var user = await _dataContext.Users.FindAsync(userName);

        _ = user ?? throw new UserNotFoundException();

        _dataContext.Users.Remove(user);
        await _dataContext.SaveChangesAsync();

        return $"User '{userName}' Successfully Deleted.";
    }

    public async Task<UserDetailDto> GetAsync(string userName)
    {
        var user = await _dataContext.Users.FindAsync(userName);

        _ = user ?? throw new UserNotFoundException();

        return user.Adapt<UserDetailDto>();
    }

    public async Task<List<UserDetailDto>> GetAsync()
    {
        var users = await _dataContext.Users.ToListAsync();

        return users.Adapt<List<UserDetailDto>>();
    }

    public async Task<string> UnlockAsync(string userName)
    {
        var user = await _dataContext.Users.FindAsync(userName);

        _ = user ?? throw new UserNotFoundException();

        user.FailedLoginCount = 0;
        user.IsLockedOut = false;

        await _dataContext.SaveChangesAsync();

        return $"User '{userName}' Successfully Unlocked.";
    }

    public async Task<string> UpdateAsync(UserDto request, string username)
    {
        var user = await _dataContext.Users.FindAsync(username);

        _ = user ?? throw new UserNotFoundException();

        CreatePasswordHash(request.Password, out var passwordHash, out var passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        await _dataContext.SaveChangesAsync();

        return $"User '{username}' Successfully Updated.";
    }
}
