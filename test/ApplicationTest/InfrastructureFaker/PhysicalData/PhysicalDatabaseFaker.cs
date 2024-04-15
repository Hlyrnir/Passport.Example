using Domain.Interface.PhysicalData;

namespace ApplicationTest.InfrastructureFaker.PhysicalData
{
    internal sealed class PhysicalDatabaseFaker
    {
        public IDictionary<Guid, IPhysicalDimension> PhysicalDimension { get; } = new Dictionary<Guid, IPhysicalDimension>();
        public IDictionary<Guid, ITimePeriod> TimePeriod { get; } = new Dictionary<Guid, ITimePeriod>();
    }
}
