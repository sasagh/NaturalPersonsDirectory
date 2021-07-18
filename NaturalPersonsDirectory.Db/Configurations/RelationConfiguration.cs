using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaturalPersonsDirectory.Models;

namespace NaturalPersonsDirectory.Db.Configurations
{
    public class RelationConfiguration : IEntityTypeConfiguration<Relation>
    {
        public void Configure(EntityTypeBuilder<Relation> builder)
        {
            builder.HasKey(relation => relation.Id);

            builder
                .HasOne(relation => relation.From)
                .WithMany()
                .HasForeignKey(relation => relation.FromId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(relation => relation.To)
                .WithMany()
                .HasForeignKey(relation => relation.ToId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
