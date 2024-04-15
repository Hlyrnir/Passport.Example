using Domain.Interface.PhysicalData;

namespace Domain.Abstraction
{
    public abstract class PhysicalQuantity<T>
    {
        private string sConcurrencyStamp;

        private Guid guId;

        public PhysicalQuantity()
        {
            this.sConcurrencyStamp = Guid.Empty.ToString();
            this.guId = Guid.Empty;
        }

        public string ConcurrencyStamp
        { get { return sConcurrencyStamp; } private set { sConcurrencyStamp = value; } } //init { sConcurrencyStamp = value; } }
        public Guid Id
        { get { return guId; } private set { guId = value; } } //init { guId = value; } }


        public abstract IPhysicalDimension Dimension { get; protected set; }
        public abstract T Magnitude { get; set; }

        public abstract bool ChangePhysicalDimension(IPhysicalDimension pdPhysicalDimension);
	}
}