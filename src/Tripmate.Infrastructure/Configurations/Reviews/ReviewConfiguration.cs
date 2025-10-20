using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Infrastructure.Configurations.Reviews
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Rating)
                .IsRequired();

            builder.Property(r => r.Comment)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(r => r.ReviewDate)
                .IsRequired();

            builder.Property(r => r.UserId)
                .IsRequired();

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

            // Configure the relationship with Restaurant
            builder.HasOne(r => r.Restaurant)
                .WithMany()
                .HasForeignKey(r => r.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship with Hotel
            builder.HasOne(r => r.Hotel)
                .WithMany()
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create indexes for better query performance
            builder.HasIndex(r => r.AttractionId);
            builder.HasIndex(r => r.RestaurantId);
            builder.HasIndex(r => r.HotelId);
            builder.HasIndex(r => r.UserId);
            builder.HasIndex(r => r.ReviewDate);

            // Ensure only one foreign key is set - a review can only be for one entity type
            builder.ToTable(t => t.HasCheckConstraint(
                "CK_Review_OneEntityType",
                "([AttractionId] IS NOT NULL AND [RestaurantId] IS NULL AND [HotelId] IS NULL) OR " +
                "([AttractionId] IS NULL AND [RestaurantId] IS NOT NULL AND [HotelId] IS NULL) OR " +
                "([AttractionId] IS NULL AND [RestaurantId] IS NULL AND [HotelId] IS NOT NULL)"
            ));

            // Add query filter for Review to match ApplicationUser's soft delete filter
            // This ensures that reviews are only shown for non-deleted users
            builder.HasQueryFilter(r => !r.User.IsDeleted);
        }
    }
}