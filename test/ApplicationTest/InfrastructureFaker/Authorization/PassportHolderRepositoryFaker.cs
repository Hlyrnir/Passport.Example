using Application.Common.Result.Repository;
using Application.Interface.Passport;
using ApplicationTest.Error;
using Domain.Interface.Authorization;
using DomainFaker;

namespace ApplicationTest.InfrastructureFaker.Authorization
{
    internal sealed class PassportHolderRepositoryFaker : IPassportHolderRepository
    {
        private readonly IPassportSetting ppSetting;

        private IDictionary<Guid, IPassportHolder> dictHolder;

        public PassportHolderRepositoryFaker(AuthorizationDatabaseFaker dbFaker, IPassportSetting ppSetting)
        {
            this.ppSetting = ppSetting;

            dictHolder = dbFaker.Holder;
        }

        public Task<RepositoryResult<bool>> DeleteAsync(IPassportHolder ppHolder, CancellationToken tknCancellation)
        {
            if (dictHolder.ContainsKey(ppHolder.Id) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportHolder.NotFound));

            return Task.FromResult(new RepositoryResult<bool>(dictHolder.Remove(ppHolder.Id)));
        }

        public Task<RepositoryResult<bool>> ExistsAsync(Guid guHolderId, CancellationToken tknCancellation)
        {
            return Task.FromResult(new RepositoryResult<bool>(dictHolder.ContainsKey(guHolderId)));
        }

        public Task<RepositoryResult<IPassportHolder>> FindByIdAsync(Guid guHolderId, CancellationToken tknCancellation)
        {
            dictHolder.TryGetValue(guHolderId, out IPassportHolder? ppHolderInRepository);

            if (ppHolderInRepository is null)
                return Task.FromResult(new RepositoryResult<IPassportHolder>(TestError.Repository.PassportHolder.NotFound));

            IPassportHolder ppHolder = DataFaker.PassportHolder.Clone(ppHolderInRepository, ppSetting);

            return Task.FromResult(new RepositoryResult<IPassportHolder>(ppHolder));
        }

        public Task<RepositoryResult<bool>> InsertAsync(IPassportHolder ppHolder, DateTimeOffset dtCreatedAt, CancellationToken tknCancellation)
        {
            if (dictHolder.ContainsKey(ppHolder.Id) == true)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportHolder.Exists));

            bool bResult = dictHolder.TryAdd(ppHolder.Id, ppHolder);

            return Task.FromResult(new RepositoryResult<bool>(bResult));
        }

        public Task<RepositoryResult<bool>> UpdateAsync(IPassportHolder ppHolder, DateTimeOffset dtEditedAt, CancellationToken tknCancellation)
        {
            if (dictHolder.ContainsKey(ppHolder.Id) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportHolder.NotFound));

            dictHolder[ppHolder.Id] = DataFaker.PassportHolder.Clone(ppHolder, ppSetting, true);

            return Task.FromResult(new RepositoryResult<bool>(true));
        }
    }
}
