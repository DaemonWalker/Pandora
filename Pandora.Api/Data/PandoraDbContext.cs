using Microsoft.EntityFrameworkCore;
using Pandora.Api.Entity;

namespace Pandora.Api.Data;

public class PandoraDbContext : DbContext
{
    public PandoraDbContext(DbContextOptions options)
        : base(options) { }

    public DbSet<ConfigurationEntity> Configurations { get; set; }
}
