using Microsoft.EntityFrameworkCore;
using Pandora.Api.Entity;

namespace Pandora.Api.Data;

public class PandoraDbContext : DbContext
{
    public DbSet<ConfigurationEntity> Configurations { get; set; }
}