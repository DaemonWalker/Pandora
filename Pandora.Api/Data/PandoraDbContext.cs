using Microsoft.EntityFrameworkCore;
using Pandora.Api.Entity;

namespace Pandora.Api.Data;

public class PandoraDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<ConfigurationEntity> Configurations { get; set; }
}
