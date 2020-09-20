using Microsoft.EntityFrameworkCore;
using NaturalPersonsDirectory.Common;
using NaturalPersonsDirectory.Db.Configurations;
using NaturalPersonsDirectory.Models;

namespace NaturalPersonsDirectory.Db
{
    public class NaturalPersonsDirectoryDbContext : DbContext
    {
        public NaturalPersonsDirectoryDbContext()
            : base(new DbContextOptionsBuilder<NaturalPersonsDirectoryDbContext>().UseSqlServer(GlobalVariables.DbConnectionString).Options)
        {
        }

        public NaturalPersonsDirectoryDbContext(DbContextOptions<NaturalPersonsDirectoryDbContext> options)
            : base(options)
        {
        }

        public DbSet<NaturalPerson> NaturalPersons { get; set; }
        public DbSet<Relation> Relations { get; set; }

        public DbSet<RelationId> RelationIds { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new NaturalPersonConfiguration());
            builder.ApplyConfiguration(new RelationConfiguration());
            builder.ApplyConfiguration(new RelationIdConfiguration());
        }
    }
}
