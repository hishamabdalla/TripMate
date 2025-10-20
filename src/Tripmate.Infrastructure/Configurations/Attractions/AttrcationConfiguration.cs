using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tripmate.Domain.Entities.Models;
using Tripmate.Domain.Enums;

namespace Tripmate.Infrastructure.Configurations.Attractions
{
    public class AttrcationConfiguration : IEntityTypeConfiguration<Attraction>
    {
        public void Configure(EntityTypeBuilder<Attraction> builder)
        {
            builder.Property(A => A.Type)
                .HasConversion(new EnumToStringConverter<AttractionType>()); // Store enum as string

            builder.HasOne(a=>a.Region)
                .WithMany(r=>r.Attractions)
                .HasForeignKey(a=>a.RegionId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
