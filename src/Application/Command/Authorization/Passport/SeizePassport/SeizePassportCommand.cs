using Application.Interface.Message;
using Application.Interface.Result;
using Mediator;

namespace Application.Command.Authorization.Passport.SeizePassport
{
	public sealed class SeizePassportCommand : ICommand<IMessageResult<bool>>, IRestrictedAuthorization, IVerifiedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }

		public required Guid PassportIdToSeize { get; init; }
	}
}