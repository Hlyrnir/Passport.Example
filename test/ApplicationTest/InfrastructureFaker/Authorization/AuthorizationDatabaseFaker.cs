using Domain.Interface.Authorization;

namespace ApplicationTest.InfrastructureFaker.Authorization
{
    internal sealed class AuthorizationDatabaseFaker
    {
        public IDictionary<Guid, IPassport> Passport { get; } = new Dictionary<Guid, IPassport>();
        public IDictionary<Guid, IPassportHolder> Holder { get; } = new Dictionary<Guid, IPassportHolder>();

        public IDictionary<Guid, IPassportToken> Token { get; } = new Dictionary<Guid, IPassportToken>();
        public IDictionary<Guid, IPassportCredential> Credential { get; } = new Dictionary<Guid, IPassportCredential>();
        public IDictionary<Guid, int> FailedAttemptCounter { get; } = new Dictionary<Guid, int>();

        public IDictionary<Guid, IPassportVisa> Visa { get; } = new Dictionary<Guid, IPassportVisa>();
        public IDictionary<Guid, IList<Guid>> VisaRegister { get; } = new Dictionary<Guid, IList<Guid>>();
    }
}