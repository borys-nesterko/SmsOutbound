using FluentAssertions;
using SmsOutbound.Application.Validators;
using SmsOutbound.Core.Abstractions;
using Xunit;

namespace SmsOutbound.UnitTests.Application;

public class PhoneNumberValidatorTests
{
	private readonly IPhoneNumberValidator _sut;

	public PhoneNumberValidatorTests()
	{
		_sut = new PhoneNumberValidator();
	}

	[Fact]
	public void IsValid_ShouldReturnTrue_WhenPhoneIsValid()
	{
		//Arrange
		var phoneNumber = "+48571870938";

		//Act
		var actual = _sut.IsValid(phoneNumber);

		//Arrange
		actual.Should().BeTrue();
	}
	
	[Fact]
	public void IsValid_ShouldReturnFalse_WhenPhoneIsNotValid()
	{
		//Arrange
		var phoneNumber = "+485";

		//Act
		var actual = _sut.IsValid(phoneNumber);

		//Arrange
		actual.Should().BeFalse();
	}
}