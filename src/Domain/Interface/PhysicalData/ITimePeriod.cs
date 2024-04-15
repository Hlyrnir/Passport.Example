using Domain.Interface.Transfer;

namespace Domain.Interface.PhysicalData
{
    public interface ITimePeriod : IPhysicalQuantity<double[]>
    {
        double Offset { get; set; }

        ITimePeriodTransferObject WriteTo<T>() where T : ITimePeriodTransferObject, new();
	}
}