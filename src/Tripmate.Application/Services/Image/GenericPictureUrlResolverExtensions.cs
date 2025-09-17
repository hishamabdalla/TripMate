using AutoMapper;
using Microsoft.AspNetCore.Http;
using Tripmate.Domain.Interfaces;

namespace Tripmate.Application.Services.Image
{
    /// <summary>
    /// Extension methods to help configure the generic picture URL resolver in AutoMapper profiles
    /// </summary>
    public static class GenericPictureUrlResolverExtensions
    {
        /// <summary>
        /// Configures a property to use the generic picture URL resolver
        /// </summary>
        /// <typeparam name="TSource">Source entity type that implements IHasImageUrl</typeparam>
        /// <typeparam name="TDestination">Destination DTO type</typeparam>
        /// <param name="memberOptions">The member configuration options</param>
        /// <param name="httpContextAccessor">HTTP context accessor service</param>
        /// <param name="imageFolderPath">The image folder path (e.g., "Countries", "Attractions")</param>
        /// <returns>The member configuration options for method chaining</returns>
        public static void MapImageUrl<TSource, TDestination>(
            this IMemberConfigurationExpression<TSource, TDestination, string> memberOptions,
            string imageFolderPath)
            where TSource : class, IHasImageUrl
        {
            memberOptions.MapFrom(new GenericPictureUrlResolver<TSource, TDestination>(imageFolderPath: imageFolderPath));

        }
    }
}