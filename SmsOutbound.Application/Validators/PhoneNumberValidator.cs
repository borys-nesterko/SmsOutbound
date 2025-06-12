using SmsOutbound.Core.Abstractions;

namespace SmsOutbound.Application.Validators;

public sealed class PhoneNumberValidator : IPhoneNumberValidator
{
	/// <summary>
	/// Validates the phone number format.
	/// </summary>
	/// <param name="phoneNumber">The phone number to validate.</param>
	/// <returns>True if the phone number is valid, otherwise false.</returns>
	public bool IsValid(string phoneNumber)
	{
		// Validation might be enhanced with more complex rules
		return !string.IsNullOrWhiteSpace(phoneNumber) &&
			   phoneNumber.StartsWith("+") &&
			   phoneNumber.Length >= 10 &&
			   phoneNumber.Length <= 15 &&
			   phoneNumber.Skip(1).All(char.IsDigit);
	}
}