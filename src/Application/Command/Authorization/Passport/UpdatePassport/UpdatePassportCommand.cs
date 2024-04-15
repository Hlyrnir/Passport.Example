using Application.Interface.Message;
using Application.Interface.Result;
using Mediator;

namespace Application.Command.Authorization.Passport.UpdatePassport
{
	public sealed class UpdatePassportCommand : ICommand<IMessageResult<bool>>, IRestrictedAuthorization, IVerifiedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }

		public required Guid PassportIdToUpdate { get; init; }
		public required DateTimeOffset ExpiredAt { get; init; }
		public required bool IsEnabled { get; init; }
		public required bool IsAuthority { get; init; }
		public required DateTimeOffset LastCheckedAt { get; init; }
		public required Guid LastCheckedBy { get; init; }
		public required IEnumerable<Guid> PassportVisaId { get; init; }
	}
}