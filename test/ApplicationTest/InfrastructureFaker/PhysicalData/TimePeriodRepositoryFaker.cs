using Application.Common.Result.Repository;
using Application.Interface.PhysicalData;
using ApplicationTest.Error;
using Domain.Interface.PhysicalData;
using DomainFaker;

namespace ApplicationTest.InfrastructureFaker.PhysicalData
{
    internal sealed class TimePeriodRepositoryFaker : ITimePeriodRepository
    {
        private readonly IDictionary<Guid, IPhysicalDimension> dictPhysicalDimension;
        private readonly IDictionary<Guid, ITimePeriod> dictTimePeriod;

        public TimePeriodRepositoryFaker(PhysicalDatabaseFaker dbFaker)
        {
            this.dictPhysicalDimension = dbFaker.PhysicalDimension;
            this.dictTimePeriod = dbFaker.TimePeriod;
        }

        public Task<RepositoryResult<bool>> DeleteAsync(ITimePeriod pdTimePeriod, CancellationToken tknCancellation)
        {
            if (dictTimePeriod.ContainsKey(pdTimePeriod.Id) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.TimePeriod.NotFound));

            return Task.FromResult(new RepositoryResult<bool>(dictTimePeriod.Remove(pdTimePeriod.Id)));
        }

        public Task<RepositoryResult<bool>> ExistsAsync(Guid guTimePeriodId, CancellationToken tknCancellation)
        {
            return Task.FromResult(new RepositoryResult<bool>(dictTimePeriod.ContainsKey(guTimePeriodId)));
        }

        public Task<RepositoryResult<ITimePeriod>> FindByIdAsync(Guid guId, CancellationToken tknCancellation)
        {
            dictTimePeriod.TryGetValue(guId, out ITimePeriod? pdTimePeriodInRepository);

            if (pdTimePeriodInRepository is null)
                return Task.FromResult(new RepositoryResult<ITimePeriod>(TestError.Repository.TimePeriod.NotFound));

            if (dictPhysicalDimension.TryGetValue(pdTimePeriodInRepository.PhysicalDimensionId, out IPhysicalDimension? pdPhysicalDimensionInRepository) == false)
                return Task.FromResult(new RepositoryResult<ITimePeriod>(TestError.Repository.PhysicalDimension.NotFound));

            ITimePeriod pdTimePeriod = DataFaker.TimePeriod.Clone(pdTimePeriodInRepository);

            return Task.FromResult(new RepositoryResult<ITimePeriod>(pdTimePeriod));
        }

        public Task<RepositoryResult<IEnumerable<ITimePeriod>>> FindByFilterAsync(ITimePeriodByFilterOption optFilter, CancellationToken tknCancellation)
        {
			IList<ITimePeriod> lstTimePeriod = new List<ITimePeriod>();

			foreach (ITimePeriod pdPhysicalDimension in dictTimePeriod.Values)
			{
				if (optFilter.PhysicalDimensionId is null || optFilter.PhysicalDimensionId == pdPhysicalDimension.PhysicalDimensionId)
					lstTimePeriod.Add(pdPhysicalDimension);
			}

			return Task.FromResult(new RepositoryResult<IEnumerable<ITimePeriod>>(lstTimePeriod.Skip((optFilter.Page + (-1)) * optFilter.PageSize).Take(optFilter.PageSize).AsEnumerable()));
		}

        public Task<RepositoryResult<bool>> InsertAsync(ITimePeriod pdTimePeriod, DateTimeOffset dtCreatedAt, CancellationToken tknCancellation)
        {
            if (dictTimePeriod.ContainsKey(pdTimePeriod.Id) == true)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportHolder.Exists));

            bool bResult = dictTimePeriod.TryAdd(pdTimePeriod.Id, pdTimePeriod);

            return Task.FromResult(new RepositoryResult<bool>(bResult));
        }

        public Task<RepositoryResult<int>> QuantityByFilterAsync(ITimePeriodByFilterOption optFilter, CancellationToken tknCancellation)
        {
            int iQuantity = dictTimePeriod.Count;

            return Task.FromResult(new RepositoryResult<int>(iQuantity));
        }

        public Task<RepositoryResult<bool>> UpdateAsync(ITimePeriod pdTimePeriod, DateTimeOffset dtUpdatedAt, CancellationToken tknCancellation)
        {
            if (dictTimePeriod.ContainsKey(pdTimePeriod.Id) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportHolder.NotFound));

            if (dictPhysicalDimension.TryGetValue(pdTimePeriod.PhysicalDimensionId, out IPhysicalDimension? pdPhysicalDimension) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportHolder.NotFound));

            dictTimePeriod[pdTimePeriod.Id] = DataFaker.TimePeriod.Clone(pdTimePeriod, true);

            return Task.FromResult(new RepositoryResult<bool>(true));
        }
    }
}