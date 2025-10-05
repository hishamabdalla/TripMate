using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Infrastructure.Configurations.Reviews
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            // Configure the relationship with ApplicationUser
            builder.HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship with Attraction
            builder.HasOne(r => r.Attraction)
                .WithMany(a => a.Reviews)
                .HasForeignKey(r => r.AttractionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add query filter for Review to match ApplicationUser's soft delete filter
            // This ensures that reviews are only shown for non-deleted users
            builder.HasQueryFilter(r => !r.User.IsDeleted);
        }
    }
}