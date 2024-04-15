using Application.Common.Result.Repository;
using Application.Interface.Passport;
using ApplicationTest.Error;
using Domain.Interface.Authorization;
using DomainFaker;

namespace ApplicationTest.InfrastructureFaker.Authorization
{
    internal sealed class PassportTokenRepositoryFaker : IPassportTokenRepository
    {
        private readonly IPassportSetting ppSetting;

        private IDictionary<Guid, IPassportToken> dictToken;
        private IDictionary<Guid, IPassportCredential> dictCredential;
        private IDictionary<Guid, int> dictFailedAttemptCounter;

        public PassportTokenRepositoryFaker(AuthorizationDatabaseFaker dbFaker, IPassportSetting ppSetting)
        {
            this.ppSetting = ppSetting;

            dictToken = dbFaker.Token;
            dictCredential = dbFaker.Credential;
            dictFailedAttemptCounter = dbFaker.FailedAttemptCounter;
        }

        public Task<RepositoryResult<bool>> CredentialAtProviderExistsAsync(string sCredential, string sProvider, CancellationToken tknCancellation)
        {
            foreach (KeyValuePair<Guid, IPassportCredential> kvpCredential in dictCredential)
            {
                if (kvpCredential.Value.Credential == sCredential
                    && kvpCredential.Value.Provider == sProvider)
                    return Task.FromResult(new RepositoryResult<bool>(true));
            }

            return Task.FromResult(new RepositoryResult<bool>(false));
        }

        public Task<RepositoryResult<bool>> DeleteAsync(IPassportToken ppToken, CancellationToken tknCancellation)
        {
            if (dictToken.ContainsKey(ppToken.Id) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportToken.NotFound));

            if (dictCredential.Remove(ppToken.Id) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportToken.Credential.NotFound));

            if (dictFailedAttemptCounter.Remove(ppToken.Id) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportToken.FailedAttemptCounter.NotFound));

            if (dictToken.Remove(ppToken.Id) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportToken.NotFound));

            return Task.FromResult(new RepositoryResult<bool>(true));
        }

        public Task<RepositoryResult<bool>> EnableTwoFactorAuthenticationAsync(IPassportToken ppToken, bool bIsEnabled, DateTimeOffset dtEditedAt, CancellationToken tknCancellation)
        {
            if (dictToken.TryGetValue(ppToken.Id, out IPassportToken? ppTokenInDictionary) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportToken.NotFound));

            bool bResult = ppTokenInDictionary.TryEnableTwoFactorAuthentication(bIsEnabled);

            if (bResult == true)
                dictToken[ppToken.Id] = DataFaker.PassportToken.Clone(ppTokenInDictionary, true);

            return Task.FromResult(new RepositoryResult<bool>(bResult));
        }

        public Task<RepositoryResult<bool>> ExistsAsync(Guid guTokenId, CancellationToken tknCancellation)
        {
            return Task.FromResult(new RepositoryResult<bool>(dictToken.TryGetValue(guTokenId, out _)));
        }

        public Task<RepositoryResult<IPassportToken>> FindTokenByCredentialAsync(IPassportCredential ppCredential, DateTimeOffset dtAttemptedAt, CancellationToken tknCancellation)
        {
            foreach (KeyValuePair<Guid, IPassportCredential> kvpCredential in dictCredential)
            {
                if (ppCredential.Credential == kvpCredential.Value.Credential
                    && ppCredential.Provider == kvpCredential.Value.Provider
                    && ppCredential.Signature == kvpCredential.Value.Signature)
                {
                    if (dictToken.TryGetValue(kvpCredential.Key, out IPassportToken? ppTokenInDictionary) == false)
                        return Task.FromResult(new RepositoryResult<IPassportToken>(TestError.Repository.PassportToken.NotFound));

                    if (dictFailedAttemptCounter.TryGetValue(kvpCredential.Key, out _) == false)
                        return Task.FromResult(new RepositoryResult<IPassportToken>(TestError.Repository.PassportToken.FailedAttemptCounter.NotFound));

                    IPassportToken ppToken = DataFaker.PassportToken.Clone(ppTokenInDictionary, bResetRefreshToken: true);

                    dictFailedAttemptCounter[kvpCredential.Key] = 0;
                    dictToken[kvpCredential.Key] = ppToken;

                    return Task.FromResult(new RepositoryResult<IPassportToken>(ppToken));
                }
            }

            return Task.FromResult(new RepositoryResult<IPassportToken>(TestError.Repository.PassportToken.Credential.NotFound));
        }

        public Task<RepositoryResult<IPassportToken>> FindTokenByRefreshTokenAsync(Guid guPassportId, string sProvider, string sRefreshToken, DateTimeOffset dtAttemptedAt, CancellationToken tknCancellation)
        {
            Guid guTokenId = Guid.Empty;

            foreach (KeyValuePair<Guid, IPassportToken> kvpToken in dictToken)
            {
                if (kvpToken.Value.PassportId == guPassportId)
                    guTokenId = kvpToken.Key;
            }

            if (dictToken.TryGetValue(guTokenId, out IPassportToken? ppTokenInDictionary) == false)
                return Task.FromResult(new RepositoryResult<IPassportToken>(TestError.Repository.PassportToken.NotFound));

            if (ppTokenInDictionary.Provider == sProvider
                && ppTokenInDictionary.RefreshToken == sRefreshToken)
            {
                if (dictFailedAttemptCounter.TryGetValue(guTokenId, out _) == false)
                    return Task.FromResult(new RepositoryResult<IPassportToken>(TestError.Repository.PassportToken.FailedAttemptCounter.NotFound));

                IPassportToken ppToken = DataFaker.PassportToken.Clone(ppTokenInDictionary, bResetRefreshToken: true);

                dictFailedAttemptCounter[guTokenId] = 0;
                dictToken[guTokenId] = ppToken;

                return Task.FromResult(new RepositoryResult<IPassportToken>(ppToken));
            }

            return Task.FromResult(new RepositoryResult<IPassportToken>(TestError.Repository.PassportToken.RefreshToken.NotFound));
        }

        public Task<RepositoryResult<bool>> InsertAsync(IPassportToken ppToken, IPassportCredential ppCredential, DateTimeOffset dtCreatedAt, CancellationToken tknCancellation)
        {
            if (dictToken.ContainsKey(ppToken.Id) == true)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportToken.Exists));

            if (dictCredential.TryAdd(ppToken.Id, ppCredential) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportToken.Credential.Exists));

            if (dictFailedAttemptCounter.TryAdd(ppToken.Id, 0) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportToken.FailedAttemptCounter.NotAdded));

            if (dictToken.TryAdd(ppToken.Id, ppToken) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportToken.Exists));

            return Task.FromResult(new RepositoryResult<bool>(true));
        }

        public Task<RepositoryResult<bool>> ResetCredentialAsync(IPassportToken ppToken, IPassportCredential ppCredentialToApply, DateTimeOffset dtResetAt, CancellationToken tknCancellation)
        {
            if (dictToken.ContainsKey(ppToken.Id) == false)
                return Task.FromResult(new RepositoryResult<bool>(TestError.Repository.PassportToken.NotFound));

            dictCredential[ppToken.Id] = DataFaker.PassportCredential.Create(ppCredentialToApply.Credential, ppCredentialToApply.Signature);

            return Task.FromResult(new RepositoryResult<bool>(true));
        }

        public Task<RepositoryResult<int>> VerifyCredentialAsync(IPassportCredential ppCredential, DateTimeOffset dtVerifiedAt, CancellationToken tknCancellation)
        {
            int iActualCount = ppSetting.MaximalAllowedAccessAttempt;

            foreach (KeyValuePair<Guid, IPassportCredential> kvpCredential in dictCredential)
            {
                if (dictFailedAttemptCounter.TryGetValue(kvpCredential.Key, out iActualCount) == false)
                    return Task.FromResult(new RepositoryResult<int>(TestError.Repository.PassportToken.FailedAttemptCounter.NotFound));

                if (kvpCredential.Value.Credential == ppCredential.Credential
                    && kvpCredential.Value.Provider == ppCredential.Provider
                    && kvpCredential.Value.Signature != ppCredential.Signature)
                {
                    if (iActualCount < ppSetting.MaximalAllowedAccessAttempt)
                    {
                        iActualCount++;
                        dictFailedAttemptCounter[kvpCredential.Key] = iActualCount;
                    }
                }
            }

            return Task.FromResult(new RepositoryResult<int>(ppSetting.MaximalAllowedAccessAttempt + -1 * iActualCount));
        }

        public Task<RepositoryResult<int>> VerifyRefreshTokenAsync(Guid guPassportId, string sProvider, string sRefreshToken, DateTimeOffset dtVerifiedAt, CancellationToken tknCancellation)
        {
            int iActualCount = ppSetting.MaximalAllowedAccessAttempt;

            Guid guTokenId = Guid.Empty;

            foreach (KeyValuePair<Guid, IPassportToken> kvpToken in dictToken)
            {
                if (kvpToken.Value.PassportId == guPassportId)
                    guTokenId = kvpToken.Key;
            }

            if (dictFailedAttemptCounter.TryGetValue(guTokenId, out iActualCount) == false)
                return Task.FromResult(new RepositoryResult<int>(TestError.Repository.PassportToken.FailedAttemptCounter.NotFound));

            if (dictToken.TryGetValue(guTokenId, out IPassportToken? ppTokenInDictionary) == false)
                return Task.FromResult(new RepositoryResult<int>(TestError.Repository.PassportToken.NotFound));

            if (ppTokenInDictionary.Provider == sProvider
                    && ppTokenInDictionary.RefreshToken != sRefreshToken)
            {
                if (iActualCount < ppSetting.MaximalAllowedAccessAttempt)
                {
                    iActualCount++;
                    dictFailedAttemptCounter[guTokenId] = iActualCount;
                }
            }

            return Task.FromResult(new RepositoryResult<int>(ppSetting.MaximalAllowedAccessAttempt + -1 * iActualCount));
        }
    }
}