using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Extension;
using Tripmate.Application.Services.Abstractions.Attraction;
using Tripmate.Application.Services.Abstractions.Country;
using Tripmate.Application.Services.Abstractions.Hotel;
using Tripmate.Application.Services.Abstractions.Identity;
using Tripmate.Application.Services.Abstractions.Region;
using Tripmate.Application.Services.Abstractions.Restaurant;
using Tripmate.Application.Services.Caching;
using Tripmate.Application.Services.Countries;
using Tripmate.Application.Services.Countries.DTOs;
using Tripmate.Application.Services.Identity;
using Tripmate.Application.Services.Identity.ForgotPassword;
using Tripmate.Application.Services.Identity.Login;
using Tripmate.Application.Services.Identity.RefreshTokens;
using Tripmate.Application.Services.Identity.Register;
using Tripmate.Application.Services.Identity.Register.DTOs;
using Tripmate.Application.Services.Identity.ResetPassword;
using Tripmate.Application.Services.Identity.Token;
using Tripmate.Application.Services.Identity.VerifyEmail;
using Tripmate.Application.Services.Image;
using Tripmate.Domain.AppSettings;
using Tripmate.Domain.Interfaces;
using Tripmate.Domain.Services.Interfaces.Identity;
using Xunit;

namespace Tripmate.Application.Extension.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        private IServiceCollection _services;
        private IConfiguration _configuration;

        public ServiceCollectionExtensionsTests()
        {
            _services = new ServiceCollection();

            // Setup configuration
            var configurationBuilder = new ConfigurationBuilder();
            var initialData = new List<KeyValuePair<string, string?>>
            {
                new("JwtSettings:Secret", "your-very-long-secret-key-here-for-testing-at-least-32-characters"),
                new("JwtSettings:Issuer", "TestIssuer"),
                new("JwtSettings:ExpirationHours", "24"),
                new("ConnectionStrings:Redis", "localhost:6379")
            };
            configurationBuilder.AddInMemoryCollection(initialData);
            _configuration = configurationBuilder.Build();
        }

        #region AddRegisterApplicationServices Tests

        [Fact]
        public void RegisterApplicationServices_RegistersExpectedServices()
        {
            // Act
            _services.RegisterApplicationServices(_configuration);

            // Assert - Check service registrations count
            _services.Should().NotBeEmpty();

            // Check specific service types are registered
            var serviceTypes = _services.Select(s => s.ServiceType).ToList();
            serviceTypes.Should().Contain(typeof(ITokenService));
            serviceTypes.Should().Contain(typeof(IRefreshTokenHandler));
            serviceTypes.Should().Contain(typeof(ILoginHandler));
            serviceTypes.Should().Contain(typeof(IRegisterHandler));
            serviceTypes.Should().Contain(typeof(IAuthService));
            serviceTypes.Should().Contain(typeof(IRefreshTokenHandler));
            serviceTypes.Should().Contain(typeof(IEmailHandler));
            serviceTypes.Should().Contain(typeof(IForgetPasswordHandler));
            serviceTypes.Should().Contain(typeof(IResetPasswordHandler));
            serviceTypes.Should().Contain(typeof(ICountryService));
            serviceTypes.Should().Contain(typeof(IFileService));
            serviceTypes.Should().Contain(typeof(IAttractionService));
            serviceTypes.Should().Contain(typeof(IRegionService));
            serviceTypes.Should().Contain(typeof(IRestaurantService));
            serviceTypes.Should().Contain(typeof(ICacheService));
            serviceTypes.Should().Contain(typeof(IHotelServices));
            serviceTypes.Should().Contain(typeof(IHttpContextAccessor));
            serviceTypes.Should().Contain(typeof(IMemoryCache));


        }
        #endregion


        #region AddApplicationServices_OptionsSetup Tests
        [Fact]
        public void AddApplicationServices_OptionsSetup_RegistersJwtSettings()
        {
            // Act
            _services.AddApplicationServices(_configuration);

            // Assert - Check service registrations count
            _services.Should().NotBeEmpty();

            var serviceProvider = _services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<JwtSettings>>();
            options.Should().NotBeNull();

        }
        #endregion

        #region AddApplicationServices_AddAutoMapperServices Tests
        [Fact]
        public void AddApplicationServices_AddAutoMapperServices_RegistersAutoMapper()
        {
            // Act
            _services.AddApplicationServices(_configuration);
            // Assert - Check service registrations count
            _services.Should().NotBeEmpty();
            var serviceProvider = _services.BuildServiceProvider();
            var mapper = serviceProvider.GetService<IMapper>();
            mapper.Should().NotBeNull();
        }
        #endregion


        #region AddApplicationServices_AddFluentValidation Tests
        [Fact]
        public void AddApplicationServices_AddFluentValidation_RegistersValidators()
        {
            // Act
            _services.AddApplicationServices(_configuration);
            // Assert - Check service registrations count
            _services.Should().NotBeEmpty();
            var serviceProvider = _services.BuildServiceProvider();
            var validators = serviceProvider.GetServices<IValidator>();
            validators.Should().NotBeNull();
        }


        #endregion



        
    }
}

      
