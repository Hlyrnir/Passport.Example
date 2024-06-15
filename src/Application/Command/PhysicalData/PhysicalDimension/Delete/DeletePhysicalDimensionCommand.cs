using Application.Interface.Message;
using Application.Interface.Result;
using Mediator;

namespace Application.Command.PhysicalData.PhysicalDimension.Delete
{
	public sealed class DeletePhysicalDimensionCommand : ICommand<IMessageResult<bool>>, IRestrictedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }

		public required Guid PhysicalDimensionId { get; init; } = Guid.Empty;
	}
}
