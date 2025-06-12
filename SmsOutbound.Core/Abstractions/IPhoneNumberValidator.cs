namespace SmsOutbound.Core.Abstractions;

public interface IPhoneNumberValidator
{
	/// <summary>
	/// Validates the phone number format.
	/// </summary>
	/// <param name="phoneNumber">The phone number to validate.</param>
	/// <returns>True if the phone number is valid, otherwise false.</returns>
	bool IsValid(string phoneNumber);
}