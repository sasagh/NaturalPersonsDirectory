using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaturalPersonsDirectory.Models;

namespace NaturalPersonsDirectory.Db.Configurations
{
    public class RelationIdConfiguration : IEntityTypeConfiguration<RelationId>
    {
        public void Configure(EntityTypeBuilder<RelationId> builder)
        {
            builder.HasKey(id => id.Id);

            builder
                .HasOne(relationId => relationId.Relation)
                .WithOne(relation => relation.RelationIdRef)
                .HasForeignKey<Relation>(relation => relation.RelationId);
        }
    }
}
