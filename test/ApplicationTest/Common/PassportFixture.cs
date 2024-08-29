using Application.Authorization;
using Application.Common.Authentication;
using Application.Common.Validation.Passport;
using Application.Interface.Authentication;
using Application.Interface.DataAccess;
using Application.Interface.Passport;
using Application.Interface.Time;
using Application.Interface.UnitOfWork;
using Application.Interface.Validation;
using Application.Token;
using ApplicationTest.InfrastructureFaker;
using ApplicationTest.InfrastructureFaker.Authorization;
using Domain.Interface.Authorization;
using DomainFaker;
using Microsoft.Extensions.Options;

namespace ApplicationTest.Common
{
    public sealed class PassportFixture
    {
        private readonly ITimeProvider prvTime;

        private readonly IPassportSetting ppSetting;

        private readonly IOptions<JwtTokenSetting> jwtSetting;
		private readonly IJwtTokenService jwtTokenService;

		private readonly IPassportRepository repoPassport;
        private readonly IPassportHolderRepository repoHolder;
        private readonly IPassportTokenRepository repoToken;
        private readonly IPassportVisaRepository repoVisa;

        private readonly IUnitOfWork<IPassportDataAccess> uowUnitOfWork;

        private readonly AuthorizationDatabaseFaker dbFaker;

        public PassportFixture()
        {
            prvTime = new TimeProviderFaker();

            IOptions<PassportHashSetting> ppHashSetting = Options.Create(new PassportHashSetting()
            {
                PublicKey = "THIS_IS_NOT_A_VALID_KEY"
            });

            ppSetting = DataFaker.PassportSetting.Create();

            uowUnitOfWork = new PassportUnitOfWorkFaker();

            dbFaker = new AuthorizationDatabaseFaker();

            repoPassport = new PassportRepositoryFaker(dbFaker);
            repoHolder = new PassportHolderRepositoryFaker(dbFaker, ppSetting);
            repoToken = new PassportTokenRepositoryFaker(dbFaker, ppSetting);
            repoVisa = new PassportVisaRepositoryFaker(dbFaker);

            jwtSetting = Options.Create(new JwtTokenSetting()
            {
                SecretKey = "LeD@IIEk0$T~zUxGjD,J09SOfV&v7$3R",
                Audience = "TEST_AUDIENCE",
                Issuer = "TEST_ISSUER"
            });

            jwtTokenService = new JwtTokenService(prvTime, jwtSetting);
        }

        public ITimeProvider TimeProvider { get => prvTime; }
        public IPassportRepository PassportRepository { get => repoPassport; }
        public IPassportHolderRepository PassportHolderRepository { get => repoHolder; }
        public IPassportTokenRepository PassportTokenRepository { get => repoToken; }
        public IPassportVisaRepository PassportVisaRepository { get => repoVisa; }
		public IUnitOfWork<IPassportDataAccess> UnitOfWork { get => uowUnitOfWork; }
		public IPassportSetting PassportSetting { get => ppSetting; }
        public IPassportValidation PassportValidation { get => new PassportValidation(ppSetting); }
        public IOptions<JwtTokenSetting> JwtTokenSetting { get => jwtSetting; }
        internal IJwtTokenService JwtTokenService { get => jwtTokenService; }
    }
}
