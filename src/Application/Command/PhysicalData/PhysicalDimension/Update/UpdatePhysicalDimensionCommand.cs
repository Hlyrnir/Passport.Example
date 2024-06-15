using Application.Interface.Message;
using Application.Interface.Result;
using Mediator;

namespace Application.Command.PhysicalData.PhysicalDimension.Update
{
	public sealed class UpdatePhysicalDimensionCommand : ICommand<IMessageResult<bool>>, IRestrictedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }

		
		public required float ExponentOfAmpere { get; init; } = 0.0f;
		public required float ExponentOfCandela { get; init; } = 0.0f;
		public required float ExponentOfKelvin { get; init; } = 0.0f;
		public required float ExponentOfKilogram { get; init; } = 0.0f;
		public required float ExponentOfMetre { get; init; } = 0.0f;
		public required float ExponentOfMole { get; init; } = 0.0f;
		public required float ExponentOfSecond { get; init; } = 0.0f;
        public required string ConcurrencyStamp { get; init; }
        public required double ConversionFactorToSI { get; init; } = 1.0;
		public required string CultureName { get; init; } = "en-GB";
		public required Guid PhysicalDimensionId { get; init; } = Guid.Empty;
		public required string Name { get; init; } = "None";
		public required string Symbol { get; init; } = "--";
		public required string Unit { get; init; } = "--";
	}
}
