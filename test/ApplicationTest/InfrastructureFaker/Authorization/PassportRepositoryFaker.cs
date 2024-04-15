using Application.Common.Result.Repository;
using Application.Interface.Passport;
using ApplicationTest.Error;
using Domain.Interface.Authorization;
using DomainFaker;

namespace ApplicationTest.InfrastructureFaker.Authorization
{
    internal sealed class PassportRepositoryFaker : IPassportRepository
    {
        private IDictionary<Guid, IPassport> dictPassport;
        private IDictionary<Guid, IPassportVisa> dictVisa;

        private IDictionary<Guid, IList<Guid>> dictVisaRegister;

        public PassportRepositoryFaker(AuthorizationDatabaseFaker dbFaker)
        {
            dictPassport = dbFaker.Passport;
            dictVisa = dbFaker.Visa;
            dictVisaRegister = dbFaker.VisaRegister;
        }

        public Task<RepositoryResult<bool>> DeleteAsync(IPassport ppPassport, CancellationToken tknCancellation)
        {
            if (dictPassport.ContainsKey(ppPassport.Id) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.Passport.NotFound));

            return Task.FromResult(new RepositoryResult<bool>(dictPassport.Remove(ppPassport.Id)));
        }

        public Task<RepositoryResult<bool>> ExistsAsync(Guid guPassportId, CancellationToken tknCancellation)
        {
            return Task.FromResult(new RepositoryResult<bool>(dictPassport.ContainsKey(guPassportId)));
        }

        public Task<RepositoryResult<IPassport>> FindByIdAsync(Guid guPassportId, CancellationToken tknCancellation)
        {
            dictPassport.TryGetValue(guPassportId, out IPassport? ppPassportInRepository);

            if (ppPassportInRepository is null)
                return Task.FromResult(new RepositoryResult<IPassport>(TestError.Repository.Passport.NotFound));

            IPassport ppPassport = DataFaker.Passport.Clone(ppPassportInRepository);

            return Task.FromResult(new RepositoryResult<IPassport>(ppPassport));
        }

        public Task<RepositoryResult<bool>> InsertAsync(IPassport ppPassport, DateTimeOffset dtCreatedAt, CancellationToken tknCancellation)
        {
            if (dictPassport.ContainsKey(ppPassport.Id) == true)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.Passport.Exists));

            bool bResult = dictPassport.TryAdd(ppPassport.Id, ppPassport);

            if (dictPassport.Keys.Contains(ppPassport.Id) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.Passport.VisaRegister));

            if (TryRegisterVisa(ppPassport.Id, ppPassport.VisaId) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportVisa.VisaRegister));

            return Task.FromResult(new RepositoryResult<bool>(bResult));
        }

        public Task<RepositoryResult<bool>> UpdateAsync(IPassport ppPassport, DateTimeOffset dtEditedAt, CancellationToken tknCancellation)
        {
            if (dictPassport.ContainsKey(ppPassport.Id) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.Passport.NotFound));

            if (TryRegisterVisa(ppPassport.Id, ppPassport.VisaId) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportVisa.NotFound));

            dictPassport[ppPassport.Id] = DataFaker.Passport.Clone(ppPassport, true);

            return Task.FromResult(new RepositoryResult<bool>(true));
        }

        private bool TryRegisterVisa(Guid guPassportId, IEnumerable<Guid> enumVisaId)
        {
            IList<Guid> lstVisa = new List<Guid>();

            foreach (Guid ppVisaId in enumVisaId)
            {
                if (dictVisa.Keys.Contains(ppVisaId) == false)
                    return false;

                lstVisa.Add(ppVisaId);
            }

            if (dictVisaRegister.ContainsKey(guPassportId) == true)
                dictVisaRegister[guPassportId] = lstVisa;
            else
                dictVisaRegister.Add(guPassportId, lstVisa);

            return true;
        }
    }
}
