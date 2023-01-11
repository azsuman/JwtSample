using JwtSample.Entities;
using Microsoft.EntityFrameworkCore;

namespace JwtSample.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
}
