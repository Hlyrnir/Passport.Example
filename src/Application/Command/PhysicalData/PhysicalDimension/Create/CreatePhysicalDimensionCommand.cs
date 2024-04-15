﻿using Application.Interface.Message;
using Application.Interface.Result;
using Mediator;

namespace Application.Command.PhysicalData.PhysicalDimension.Create
{
	public sealed class CreatePhysicalDimensionCommand : ICommand<IMessageResult<Guid>>, IRestrictedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }

		public float ExponentOfAmpere { get; init; } = 0.0f;
		public float ExponentOfCandela { get; init; } = 0.0f;
		public float ExponentOfKelvin { get; init; } = 0.0f;
		public float ExponentOfKilogram { get; init; } = 0.0f;
		public float ExponentOfMetre { get; init; } = 0.0f;
		public float ExponentOfMole { get; init; } = 0.0f;
		public float ExponentOfSecond { get; init; } = 0.0f;

		public double ConversionFactorToSI { get; init; } = 1.0;
		public string CultureName { get; init; } = "en-GB";
		public string Name { get; init; } = "None";
		public string Symbol { get; init; } = "--";
		public string Unit { get; init; } = "--";
	}
}
