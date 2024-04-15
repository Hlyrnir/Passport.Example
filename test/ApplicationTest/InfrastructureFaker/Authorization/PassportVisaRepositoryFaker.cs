using Application.Common.Result.Repository;
using Application.Interface.Passport;
using ApplicationTest.Error;
using Domain.Interface.Authorization;
using DomainFaker;

namespace ApplicationTest.InfrastructureFaker.Authorization
{
    internal sealed class PassportVisaRepositoryFaker : IPassportVisaRepository
    {
        private IDictionary<Guid, IPassportVisa> dictVisa;
        private IDictionary<Guid, IList<Guid>> dictVisaRegister;

        public PassportVisaRepositoryFaker(AuthorizationDatabaseFaker dbFaker)
        {
            dictVisa = dbFaker.Visa;
            dictVisaRegister = dbFaker.VisaRegister;
        }

		public Task<RepositoryResult<bool>> DeleteAsync(IPassportVisa ppVisa, CancellationToken tknCancellation)
        {
            if (dictVisa.ContainsKey(ppVisa.Id) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportVisa.NotFound));

            return Task.FromResult(new RepositoryResult<bool>(dictVisa.Remove(ppVisa.Id)));
        }

        public Task<RepositoryResult<bool>> ExistsAsync(Guid guVisaId, CancellationToken tknCancellation)
        {
            return Task.FromResult(new RepositoryResult<bool>(dictVisa.ContainsKey(guVisaId)));
        }

        public Task<RepositoryResult<bool>> ExistsAsync(IEnumerable<Guid> enumVisaId, CancellationToken tknCancellation)
        {
            foreach (Guid guPassportVisaId in enumVisaId)
            {
                if (dictVisa.ContainsKey(guPassportVisaId) == false)
                    return Task.FromResult(new RepositoryResult<bool>(false));
            }

            return Task.FromResult(new RepositoryResult<bool>(true));
        }

        public Task<RepositoryResult<bool>> ByNameAtLevelExistsAsync(string sName, int iLevel, CancellationToken tknCancellation)
        {
            foreach (IPassportVisa ppVisa in dictVisa.Values)
            {
                if (ppVisa.Name == sName && ppVisa.Level == iLevel)
                    return Task.FromResult(new RepositoryResult<bool>(true));
            }

            return Task.FromResult(new RepositoryResult<bool>(false));
        }

		public Task<RepositoryResult<IPassportVisa>> FindByIdAsync(Guid guVisaId, CancellationToken tknCancellation)
        {
            dictVisa.TryGetValue(guVisaId, out IPassportVisa? ppVisaInRepository);

            if (ppVisaInRepository is null)
                return Task.FromResult(new RepositoryResult<IPassportVisa>(TestError.Repository.PassportVisa.NotFound));

            IPassportVisa ppVisa = DataFaker.PassportVisa.Clone(ppVisaInRepository);

            return Task.FromResult(new RepositoryResult<IPassportVisa>(ppVisa));
        }

        public Task<RepositoryResult<IPassportVisa>> FindByNameAsync(string sName, int iLevel, CancellationToken tknCancellation)
        {
            foreach (KeyValuePair<Guid, IPassportVisa> kvpVisa in dictVisa)
            {
                if (kvpVisa.Value.Name == sName
                    && kvpVisa.Value.Level == iLevel)
                {
                    IPassportVisa ppVisa = DataFaker.PassportVisa.Clone(kvpVisa.Value);

                    return Task.FromResult(new RepositoryResult<IPassportVisa>(ppVisa));
                }

            }

            return Task.FromResult(new RepositoryResult<IPassportVisa>(TestError.Repository.PassportVisa.NotFound));
        }

        public Task<RepositoryResult<IEnumerable<IPassportVisa>>> FindByPassportAsync(Guid guPassportId, CancellationToken tknCancellation)
        {
            IList<IPassportVisa> lstVisa = new List<IPassportVisa>();

            if (dictVisaRegister.TryGetValue(guPassportId, out IList<Guid>? lstVisaId) == false)
                return Task.FromResult(new RepositoryResult<IEnumerable<IPassportVisa>>(TestError.Repository.PassportVisa.VisaRegister));

            foreach (Guid guVisaId in lstVisaId)
            {
                if (dictVisa.TryGetValue(guVisaId, out IPassportVisa? ppVisaInRepository) == true)
                {
                    IPassportVisa ppVisa = DataFaker.PassportVisa.Clone(ppVisaInRepository);
                    lstVisa.Add(ppVisa);
                }
            }

            return Task.FromResult(new RepositoryResult<IEnumerable<IPassportVisa>>(lstVisa.AsEnumerable()));
        }

        public Task<RepositoryResult<bool>> InsertAsync(IPassportVisa ppVisa, DateTimeOffset dtCreatedAt, CancellationToken tknCancellation)
        {
            if (dictVisa.ContainsKey(ppVisa.Id) == true)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportVisa.Exists));

            bool bResult = dictVisa.TryAdd(ppVisa.Id, ppVisa);

            return Task.FromResult(new RepositoryResult<bool>(bResult));
        }

        public Task<RepositoryResult<bool>> UpdateAsync(IPassportVisa ppVisa, DateTimeOffset dtEditedAt, CancellationToken tknCancellation)
        {
            if (dictVisa.ContainsKey(ppVisa.Id) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportVisa.NotFound));

            dictVisa[ppVisa.Id] = DataFaker.PassportVisa.Clone(ppVisa, true);

            return Task.FromResult(new RepositoryResult<bool>(true));
        }
    }
}
