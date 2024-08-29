using Application.Authorization;
using Application.Common.Authentication;
using Application.Common.Authorization;
using Application.Common.DataProtection;
using Application.Common.Time;
using Application.Common.Validation;
using Application.Common.Validation.Passport;
using Application.Common.Validation.PhysicalData;
using Application.Interface.Authentication;
using Application.Interface.Time;
using Application.Interface.Validation;
using Application.Token;
using Domain.Interface.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Application.ServiceCollectionExtension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplication(this IServiceCollection cltService, IConfiguration cfgConfiguration)
        {
            // Add time provider
            cltService.TryAddSingleton<ITimeProvider, TimeProvider>();

            // Add mediator
            cltService.AddMediator(
                mdtOptions =>
                {
                    mdtOptions.ServiceLifetime = ServiceLifetime.Scoped;
                });

            // Add pipeline behaviour to Mediator
            cltService.AddCommandBehaviour();
            cltService.AddQueryBehaviour();

            // Add validation for settings
            cltService.TryAddSingleton<IValidateOptions<DataProtectionSetting>, DataProtectionSettingValidation>();
            cltService.TryAddSingleton<IValidateOptions<JwtTokenSetting>, JwtTokenSettingValidation>();
            cltService.TryAddSingleton<IValidateOptions<PassportHashSetting>, PassportHashSettingValidation>();

            // Add data protection
            cltService.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(cfgConfiguration[DataProtectionSetting.KeyStoragePathName]!))
                .SetApplicationName(DataProtectionSetting.ApplicationName);
            cltService.TryAddTransient(prvService =>
            {
                return prvService.GetDataProtector(DataProtectionSetting.DataProtectorPurpose);
            });

            // Add jwt service
            cltService.TryAddScoped<IJwtTokenService, JwtTokenService>();

            cltService.TryAddTransient<IPassportCredential, PassportCredential>();

            cltService.TryAddSingleton<IPassportHasher, PassportHasher>();

            cltService.TryAddScoped<IPassportSetting, PassportSetting>();
            cltService.TryAddTransient<IPassportValidation, PassportValidation>();

            cltService.TryAddTransient<IPhysicalDataValidation, PhysicalDataValidation>();

			return cltService;
        }
    }
}