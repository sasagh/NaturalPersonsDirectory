using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NaturalPersonsDirectory.Models;

namespace NaturalPersonsDirectory.Db.Configurations
{
    public class NaturalPersonConfiguration : IEntityTypeConfiguration<NaturalPerson>
    {
        public void Configure(EntityTypeBuilder<NaturalPerson> builder)
        {
            builder.HasKey(naturalPerson => naturalPerson.NaturalPersonId);
        }
    }
}
