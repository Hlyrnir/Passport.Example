using Application.Interface.Message;
using Application.Interface.Result;
using Mediator;

namespace Application.Query.Authorization.PassportHolder.ById
{
	public sealed class PassportHolderByIdQuery : IQuery<IMessageResult<PassportHolderByIdResult>>, IRestrictedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }
		public required Guid PassportHolderId { get; init; }
	}
}
