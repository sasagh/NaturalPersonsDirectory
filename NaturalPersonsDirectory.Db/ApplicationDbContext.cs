using Microsoft.EntityFrameworkCore;
using NaturalPersonsDirectory.Db.Configurations;
using NaturalPersonsDirectory.Models;

namespace NaturalPersonsDirectory.Db
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<NaturalPerson> NaturalPersons { get; set; }

        public DbSet<Relation> Relations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new NaturalPersonConfiguration());
            builder.ApplyConfiguration(new RelationConfiguration());
        }
    }
}
