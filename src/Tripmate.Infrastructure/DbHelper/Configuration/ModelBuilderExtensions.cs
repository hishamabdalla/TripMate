using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Tripmate.Infrastructure.Data.Configuration
{
    public static class ModelBuilderExtensions
    {
        public static void ApplySoftDeleteQueryFilter(this ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;//give type of class

                var isDeletedProperty = clrType.GetProperty("IsDeleted");
                if (isDeletedProperty != null && isDeletedProperty.PropertyType == typeof(bool))
                {
                    var parameter = Expression.Parameter(clrType, "e");
                    var property = Expression.Property(parameter, "IsDeleted");
                    var condition = Expression.Equal(property, Expression.Constant(false));
                    var lambda = Expression.Lambda(condition, parameter);

                    modelBuilder.Entity(clrType).HasQueryFilter(lambda);
                }
            }
        }
    }
}
