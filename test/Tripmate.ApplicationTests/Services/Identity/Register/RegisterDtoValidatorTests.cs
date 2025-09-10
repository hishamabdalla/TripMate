using FluentAssertions;
using Tripmate.Application.Services.Identity.Register;
using Tripmate.Application.Services.Identity.Register.DTOs;
using Xunit;

namespace Tripmate.ApplicationTests.Services.Identity.Register;

public class RegisterDtoValidatorTests
{
    private readonly RegisterDtoValidator _validator = new();


    #region Valid Test Cases
    [Theory]
    [InlineData("Hisham", "hisham@gmail.com", "01008295779", "Hisham@123", "Hisham@123", "Egypt")]
    public void RegisterDto_WithValidData_ShouldPassValidation(string username, string email, string phoneNumber, string password, string confirmPassword, string country)
    {
        var dto = new RegisterDto
        {
            UserName = username,
            Email = email,
            PhoneNumber = phoneNumber,
            Password = password,
            ConfirmPassword = confirmPassword,
            Country = country
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue("because all data is valid");
        result.Errors.Should().BeEmpty();
    }
    #endregion

    #region Invalid Test Cases - Username
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("ab")] // less than 3 characters
    [InlineData("abcdefghijklmnopqrstu")] // more than 50 characters
    public void RegisterDto_WithInvalidUsername_ShouldFailValidation(string invalidUsername)
    {
        var dto = new RegisterDto
        {
            UserName = invalidUsername,
            Email = "valid@email.com",
            PhoneNumber = "201008295779",
            Password = "Hisham123",
            ConfirmPassword = "Hisham123",
            Country = "Egypt"

        };
        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse("because the username is invalid");
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterDto.UserName));

    }

    #endregion

    #region Invalid Test Cases - Email

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid-email")]
    public void RegisterDto_WithInvalidEmail_ShouldFailValidation(string? invalidEmail)
    {
        var dto = new RegisterDto
        {
            UserName = "ValidUser",
            Email = invalidEmail!,
            PhoneNumber = "201008295779",
            Password = "Hisham@123",
            ConfirmPassword = "Hisham@123",
            Country = "Egypt"
        };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse("because the email is invalid");
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterDto.Email));
    }


    #endregion


    #region Invalid Test Cases - Phone Number
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("invalid-phone")]

    public void RegisterDto_WithInvalidPhoneNumber_ShouldFailValidation(string? invalidPhoneNumber)
    {
        var dto = new RegisterDto
        {
            UserName = "validUsername",
            Email = "valid@email.com",
            PhoneNumber = invalidPhoneNumber!,
            Password = "Hisham123",
            ConfirmPassword = "Hisham123",
            Country = "Egypt"

        };
        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse("because the phone number is invalid");
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterDto.PhoneNumber));
    }
    #endregion

    #region Invalid Test Cases - Password
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("hisham")]
    [InlineData("hisham23")]
    [InlineData("HISHAM45")]

    public void RegisterDto_WithInvalidPassword_ShouldFailValidation(string invalidPassword)
    {
        var dto = new RegisterDto
        {
            UserName = "validUsername",
            Email = "valid@email.com",
            PhoneNumber = "01008295776",
            Password = invalidPassword,
            ConfirmPassword = "Hisham123",
            Country = "Egypt"
        };
        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse("because the password is invalid");
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterDto.Password));
    }

    #endregion

    #region Invalid Test Cases - Confirm Password
    [Theory]
    [InlineData("Password123", "DifferentPassword123")]
    [InlineData("Test123", "")]
    [InlineData("Test123", null)]
    public void RegisterDto_WithMismatchedConfirmPassword_ShouldFailValidation(string password, string confirmPassword)
    {
        var dto = new RegisterDto
        {
            UserName = "validUsername",
            Email = "valid@email.com",
            PhoneNumber = "01008295776",
            Password = password,
            ConfirmPassword = confirmPassword,
            Country = "Egypt"
        };
        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse("because the confirm password does not match the password");
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterDto.ConfirmPassword));
    }
    #endregion

    #region Invalid Test Cases - Country

    [Theory]
    [InlineData(null)]
    [InlineData("")]

    public void RegisterDto_WithInvalidCountry_ShouldFailValidation(string invalidCountry)
    {
        var dto = new RegisterDto
        {
            UserName = "validUsername",
            Email = "valid@email.com",
            PhoneNumber = "01008295776",
            Password = "Hisham@123",
            ConfirmPassword = "Hisham@123",
            Country = invalidCountry
        };

        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse("because country is invalid");
        result.Errors.Should().Contain(e=>e.PropertyName == nameof(RegisterDto.Country));
    }
    #endregion
}