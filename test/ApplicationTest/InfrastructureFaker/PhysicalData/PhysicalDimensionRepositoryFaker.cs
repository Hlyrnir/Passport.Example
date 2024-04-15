using Application.Common.Result.Repository;
using Application.Interface.PhysicalData;
using ApplicationTest.Error;
using Domain.Interface.PhysicalData;
using DomainFaker;

namespace ApplicationTest.InfrastructureFaker.PhysicalData
{
    internal sealed class PhysicalDimensionRepositoryFaker : IPhysicalDimensionRepository
    {
        private readonly IDictionary<Guid, IPhysicalDimension> dictPhysicalDimension;

        public PhysicalDimensionRepositoryFaker(PhysicalDatabaseFaker dbFaker)
        {
            this.dictPhysicalDimension = dbFaker.PhysicalDimension;
        }

        public Task<RepositoryResult<bool>> DeleteAsync(IPhysicalDimension pdPhysicalDimension, CancellationToken tknCancellation)
        {
            if (dictPhysicalDimension.ContainsKey(pdPhysicalDimension.Id) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PhysicalDimension.NotFound));

            return Task.FromResult(new RepositoryResult<bool>(dictPhysicalDimension.Remove(pdPhysicalDimension.Id)));
        }

        public Task<RepositoryResult<bool>> ExistsAsync(Guid guPhysicalDimensionId, CancellationToken tknCancellation)
        {
            return Task.FromResult(new RepositoryResult<bool>(dictPhysicalDimension.ContainsKey(guPhysicalDimensionId)));
        }

        public Task<RepositoryResult<IPhysicalDimension>> FindByIdAsync(Guid guId, CancellationToken tknCancellation)
        {
            dictPhysicalDimension.TryGetValue(guId, out IPhysicalDimension? pdPhysicalDimensionInRepository);

            if (pdPhysicalDimensionInRepository is null)
                return Task.FromResult(new RepositoryResult<IPhysicalDimension>(TestError.Repository.PhysicalDimension.NotFound));

            IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.Clone(pdPhysicalDimensionInRepository);

            return Task.FromResult(new RepositoryResult<IPhysicalDimension>(pdPhysicalDimension));
        }

        public Task<RepositoryResult<IEnumerable<IPhysicalDimension>>> FindByFilterAsync(IPhysicalDimensionByFilterOption optFilter, CancellationToken tknCancellation)
        {
            IList<IPhysicalDimension> lstPhysicalDimension = new List<IPhysicalDimension>();

            foreach (IPhysicalDimension pdPhysicalDimension in dictPhysicalDimension.Values)
            {
                if (optFilter.ConversionFactorToSI is null || optFilter.ConversionFactorToSI == pdPhysicalDimension.ConversionFactorToSI
                    && optFilter.CultureName is null || optFilter.CultureName is not null && pdPhysicalDimension.CultureName.Contains(optFilter.CultureName) == true
                    && optFilter.ExponentOfAmpere is null || optFilter.ExponentOfAmpere == pdPhysicalDimension.ExponentOfUnit.Ampere
                    && optFilter.ExponentOfCandela is null || optFilter.ExponentOfCandela == pdPhysicalDimension.ExponentOfUnit.Candela
                    && optFilter.ExponentOfKelvin is null || optFilter.ExponentOfKelvin == pdPhysicalDimension.ExponentOfUnit.Kelvin
                    && optFilter.ExponentOfKilogram is null || optFilter.ExponentOfKilogram == pdPhysicalDimension.ExponentOfUnit.Kilogram
                    && optFilter.ExponentOfMetre is null || optFilter.ExponentOfMetre == pdPhysicalDimension.ExponentOfUnit.Metre
                    && optFilter.ExponentOfMole is null || optFilter.ExponentOfMole == pdPhysicalDimension.ExponentOfUnit.Mole
                    && optFilter.ExponentOfSecond is null || optFilter.ExponentOfSecond == pdPhysicalDimension.ExponentOfUnit.Second
                    && optFilter.Name is null || optFilter.Name is not null && pdPhysicalDimension.Name.Contains(optFilter.Name) == true
                    && optFilter.Symbol is null || optFilter.Symbol is not null && pdPhysicalDimension.Symbol.Contains(optFilter.Symbol) == true
                    && optFilter.Unit is null || optFilter.Unit is not null && pdPhysicalDimension.Unit.Contains(optFilter.Unit) == true)
                    lstPhysicalDimension.Add(pdPhysicalDimension);
            }

            return Task.FromResult(new RepositoryResult<IEnumerable<IPhysicalDimension>>(lstPhysicalDimension.Skip((optFilter.Page + (-1)) * optFilter.PageSize).Take(optFilter.PageSize).AsEnumerable()));
        }

        public Task<RepositoryResult<bool>> InsertAsync(IPhysicalDimension pdPhysicalDimension, DateTimeOffset dtCreatedAt, CancellationToken tknCancellation)
        {
            if (dictPhysicalDimension.ContainsKey(pdPhysicalDimension.Id) == true)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportHolder.Exists));

            bool bResult = dictPhysicalDimension.TryAdd(pdPhysicalDimension.Id, pdPhysicalDimension);

            return Task.FromResult(new RepositoryResult<bool>(bResult));
        }

        public Task<RepositoryResult<int>> QuantityByFilterAsync(IPhysicalDimensionByFilterOption optFilter, CancellationToken tknCancellation)
        {
            int iQuantity = dictPhysicalDimension.Count;

            return Task.FromResult(new RepositoryResult<int>(iQuantity));
        }

        public Task<RepositoryResult<bool>> UpdateAsync(IPhysicalDimension pdPhysicalDimension, DateTimeOffset dtUpdatedAt, CancellationToken tknCancellation)
        {
            if (dictPhysicalDimension.ContainsKey(pdPhysicalDimension.Id) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportHolder.NotFound));

            dictPhysicalDimension[pdPhysicalDimension.Id] = DataFaker.PhysicalDimension.Clone(pdPhysicalDimension, true);

            return Task.FromResult(new RepositoryResult<bool>(true));
        }
    }
}