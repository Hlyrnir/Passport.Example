using Application.Interface.Message;
using Application.Interface.Result;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Command.Authorization.Passport.RegisterPassport
{
	public sealed class RegisterPassportCommand : ICommand<IMessageResult<Guid>>, IRestrictedAuthorization
	{
        public required Guid RestrictedPassportId { get; init; }
		public required Guid IssuedBy { get; init; }

		public required IPassportCredential CredentialToRegister { get; init; }

		public string CultureName { get; init; } = "en-GB";
		public string EmailAddress { get; init; } = "NO_EMAIL_ADDRESS";
		public string FirstName { get; init; } = "NO_FIRST_NAME";
		public string LastName { get; init; } = "NO_LAST_NAME";
		public string PhoneNumber { get; init; } = "NO_PHONE_NUMBER";
	}
}