using Application.Interface.Passport;
using Microsoft.Extensions.Options;
using Application.Interface.DataAccess;
using Microsoft.Extensions.Configuration;
using Domain.Interface.Authorization;
using DomainFaker;
using Infrastructure.Persistance.Authorization;
using Application.Authorization;
using InfrastructureTest.Common;

namespace InfrastructureTest.Authorization.Common
{
    public class PassportFixture
	{
		private readonly ITimeProvider prvTime;

		private readonly IConfiguration cfgConfiguration;
		private readonly IPassportSetting ppSetting;

		private readonly IPassportRepository repoPassport;
		private readonly IPassportHolderRepository repoHolder;
		private readonly IPassportTokenRepository repoToken;
		private readonly IPassportVisaRepository repoVisa;

		public PassportFixture()
		{
			prvTime = new TimeProviderFaker();

			cfgConfiguration = new ConfigurationBuilder()
				.AddInMemoryCollection(
					new[]
					{
						new KeyValuePair<string, string?>("ConnectionStrings:TestDatabase", "Data Source=D:\\Dateien\\Projekte\\CSharp\\CQRS_Prototype\\TEST_Passport.db; Mode=ReadWrite")
					})
				.Build();

			IPassportDataAccess sqlDataAccess = new PassportDataAccessFaker(cfgConfiguration, "TestDatabase");

			IOptions<PassportHashSetting> ppHashSetting = Options.Create(new PassportHashSetting()
			{
				PublicKey = "THIS_IS_NOT_A_VALID_KEY"
			});

			IPassportHasher ppHasher = new PassportHasher(ppHashSetting);

			ppSetting = DataFaker.PassportSetting.Create();

			repoPassport = new PassportRepository(sqlDataAccess);
			repoHolder = new PassportHolderRepository(sqlDataAccess, ppSetting);
			repoToken = new PassportTokenRepository(sqlDataAccess, ppSetting, ppHasher);
			repoVisa = new PassportVisaRepository(sqlDataAccess);
		}

		public ITimeProvider TimeProvider { get => prvTime; }
		public IPassportRepository PassportRepository { get => repoPassport; }
		public IPassportHolderRepository PassportHolderRepository { get => repoHolder; }
		public IPassportTokenRepository PassportTokenRepository { get => repoToken; }
		public IPassportVisaRepository PassportVisaRepository { get => repoVisa; }
		public IPassportSetting PassportSetting { get => ppSetting; }
	}
}