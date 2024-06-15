using Application.Command.Authentication.BearerTokenByCredential;
using Application.Command.Authentication.BearerTokenByRefreshToken;
using Application.Command.Authorization.Passport.Register;
using Application.Command.Authorization.Passport.Seize;
using Application.Command.Authorization.Passport.Update;
using Application.Command.Authorization.PassportHolder.ConfirmEmailAddress;
using Application.Command.Authorization.PassportHolder.ConfirmPhoneNumber;
using Application.Command.Authorization.PassportHolder.Delete;
using Application.Command.Authorization.PassportHolder.Update;
using Application.Command.Authorization.PassportVisa.Create;
using Application.Command.Authorization.PassportVisa.Delete;
using Application.Command.Authorization.PassportVisa.Update;
using Application.Command.PhysicalData.PhysicalDimension.Create;
using Application.Command.PhysicalData.PhysicalDimension.Delete;
using Application.Command.PhysicalData.PhysicalDimension.Update;
using Application.Command.PhysicalData.TimePeriod.Create;
using Application.Command.PhysicalData.TimePeriod.Delete;
using Application.Command.PhysicalData.TimePeriod.Update;
using Application.Common.Authorization;
using Application.Common.Validation;
using Application.Interface.Authorization;
using Application.Interface.Result;
using Application.Interface.Validation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Application.ServiceCollectionExtension
{
    public static class CommandBehaviorExtension
    {
        public static IServiceCollection AddCommandBehaviour(this IServiceCollection cltService)
        {
            #region Authentication - Validation
            cltService.AddScoped(typeof(IPipelineBehavior<BearerTokenByCredentialCommand, IMessageResult<string>>), typeof(MessageValidationBehaviour<BearerTokenByCredentialCommand, string>));
            cltService.TryAddTransient<IValidation<BearerTokenByCredentialCommand>, BearerTokenByCredentialValidation>();

            cltService.AddScoped(typeof(IPipelineBehavior<BearerTokenByRefreshTokenCommand, IMessageResult<string>>), typeof(MessageValidationBehaviour<BearerTokenByRefreshTokenCommand, string>));
            cltService.TryAddTransient<IValidation<BearerTokenByRefreshTokenCommand>, BearerTokenByRefreshTokenValidation>();
			#endregion

			#region Passport - Authorization
			cltService.AddScoped(typeof(IPipelineBehavior<RegisterPassportCommand, IMessageResult<Guid>>), typeof(MessageAuthorizationBehaviour<RegisterPassportCommand, Guid>));
			cltService.TryAddTransient<IAuthorization<RegisterPassportCommand>, RegisterPassportAuthorization>();

			cltService.AddScoped(typeof(IPipelineBehavior<SeizePassportCommand, IMessageResult<bool>>), typeof(MessageAuthorizationBehaviour<SeizePassportCommand, bool>));
			cltService.TryAddTransient<IAuthorization<SeizePassportCommand>, SeizePassportAuthorization>();

			cltService.AddScoped(typeof(IPipelineBehavior<UpdatePassportCommand, IMessageResult<bool>>), typeof(MessageAuthorizationBehaviour<UpdatePassportCommand, bool>));
			cltService.TryAddTransient<IAuthorization<UpdatePassportCommand>, UpdatePassportAuthorization>();
			#endregion

			#region Passport - Validation
			cltService.AddScoped(typeof(IPipelineBehavior<RegisterPassportCommand, IMessageResult<Guid>>), typeof(MessageValidationBehaviour<RegisterPassportCommand, Guid>));
			cltService.TryAddTransient<IValidation<RegisterPassportCommand>, RegisterPassportValidation>();

			cltService.AddScoped(typeof(IPipelineBehavior<SeizePassportCommand, IMessageResult<bool>>), typeof(MessageValidationBehaviour<SeizePassportCommand, bool>));
			cltService.TryAddTransient<IValidation<SeizePassportCommand>, SeizePassportValidation>();

			cltService.AddScoped(typeof(IPipelineBehavior<UpdatePassportCommand, IMessageResult<bool>>), typeof(MessageValidationBehaviour<UpdatePassportCommand, bool>));
			cltService.TryAddTransient<IValidation<UpdatePassportCommand>, UpdatePassportValidation>();
			#endregion

			#region PassportHolder - Authorization
			cltService.AddScoped(typeof(IPipelineBehavior<ConfirmEmailAddressCommand, IMessageResult<bool>>), typeof(MessageAuthorizationBehaviour<ConfirmEmailAddressCommand, bool>));
			cltService.TryAddTransient<IAuthorization<ConfirmEmailAddressCommand>, ConfirmEmailAddressAuthorization>();

			cltService.AddScoped(typeof(IPipelineBehavior<ConfirmPhoneNumberCommand, IMessageResult<bool>>), typeof(MessageAuthorizationBehaviour<ConfirmPhoneNumberCommand, bool>));
			cltService.TryAddTransient<IAuthorization<ConfirmPhoneNumberCommand>, ConfirmPhoneNumberAuthorization>();

			cltService.AddScoped(typeof(IPipelineBehavior<DeletePassportHolderCommand, IMessageResult<bool>>), typeof(MessageAuthorizationBehaviour<DeletePassportHolderCommand, bool>));
			cltService.TryAddTransient<IAuthorization<DeletePassportHolderCommand>, DeletePassportHolderAuthorization>();

			cltService.AddScoped(typeof(IPipelineBehavior<UpdatePassportHolderCommand, IMessageResult<bool>>), typeof(MessageAuthorizationBehaviour<UpdatePassportHolderCommand, bool>));
			cltService.TryAddTransient<IAuthorization<UpdatePassportHolderCommand>, UpdatePassportHolderAuthorization>();
			#endregion

			#region PassportHolder - Validation
			cltService.AddScoped(typeof(IPipelineBehavior<ConfirmEmailAddressCommand, IMessageResult<bool>>), typeof(MessageValidationBehaviour<ConfirmEmailAddressCommand, bool>));
			cltService.TryAddTransient<IValidation<ConfirmEmailAddressCommand>, ConfirmEmailAddressValidation>();

			cltService.AddScoped(typeof(IPipelineBehavior<ConfirmPhoneNumberCommand, IMessageResult<bool>>), typeof(MessageValidationBehaviour<ConfirmPhoneNumberCommand, bool>));
			cltService.TryAddTransient<IValidation<ConfirmPhoneNumberCommand>, ConfirmPhoneNumberValidation>();

			cltService.AddScoped(typeof(IPipelineBehavior<DeletePassportHolderCommand, IMessageResult<bool>>), typeof(MessageValidationBehaviour<DeletePassportHolderCommand, bool>));
			cltService.TryAddTransient<IValidation<DeletePassportHolderCommand>, DeletePassportHolderValidation>();

			cltService.AddScoped(typeof(IPipelineBehavior<UpdatePassportHolderCommand, IMessageResult<bool>>), typeof(MessageValidationBehaviour<UpdatePassportHolderCommand, bool>));
			cltService.TryAddTransient<IValidation<UpdatePassportHolderCommand>, UpdatePassportHolderValidation>();
			#endregion

			#region PassportVisa - Authorization
			cltService.AddScoped(typeof(IPipelineBehavior<CreatePassportVisaCommand, IMessageResult<Guid>>), typeof(MessageAuthorizationBehaviour<CreatePassportVisaCommand, Guid>));
			cltService.TryAddTransient<IAuthorization<CreatePassportVisaCommand>, CreatePassportVisaAuthorization>();

			cltService.AddScoped(typeof(IPipelineBehavior<DeletePassportVisaCommand, IMessageResult<bool>>), typeof(MessageAuthorizationBehaviour<DeletePassportVisaCommand, bool>));
			cltService.TryAddTransient<IAuthorization<DeletePassportVisaCommand>, DeletePassportVisaAuthorization>();

			cltService.AddScoped(typeof(IPipelineBehavior<UpdatePassportVisaCommand, IMessageResult<bool>>), typeof(MessageAuthorizationBehaviour<UpdatePassportVisaCommand, bool>));
			cltService.TryAddTransient<IAuthorization<UpdatePassportVisaCommand>, UpdatePassportVisaAuthorization>();
			#endregion

			#region PassportVisa - Validation
			cltService.AddScoped(typeof(IPipelineBehavior<CreatePassportVisaCommand, IMessageResult<Guid>>), typeof(MessageValidationBehaviour<CreatePassportVisaCommand, Guid>));
			cltService.TryAddTransient<IValidation<CreatePassportVisaCommand>, CreatePassportVisaValidation>();

			cltService.AddScoped(typeof(IPipelineBehavior<DeletePassportVisaCommand, IMessageResult<bool>>), typeof(MessageValidationBehaviour<DeletePassportVisaCommand, bool>));
			cltService.TryAddTransient<IValidation<DeletePassportVisaCommand>, DeletePassportVisaValidation>();

			cltService.AddScoped(typeof(IPipelineBehavior<UpdatePassportVisaCommand, IMessageResult<bool>>), typeof(MessageValidationBehaviour<UpdatePassportVisaCommand, bool>));
			cltService.TryAddTransient<IValidation<UpdatePassportVisaCommand>, UpdatePassportVisaValidation>();
			#endregion

			#region PhysicalDimension - Authorization
			cltService.AddScoped(typeof(IPipelineBehavior<CreatePhysicalDimensionCommand, IMessageResult<Guid>>), typeof(MessageAuthorizationBehaviour<CreatePhysicalDimensionCommand, Guid>));
			cltService.TryAddTransient<IAuthorization<CreatePhysicalDimensionCommand>, CreatePhysicalDimensionAuthorization>();

			cltService.AddScoped(typeof(IPipelineBehavior<DeletePhysicalDimensionCommand, IMessageResult<bool>>), typeof(MessageAuthorizationBehaviour<DeletePhysicalDimensionCommand, bool>));
            cltService.TryAddTransient<IAuthorization<DeletePhysicalDimensionCommand>, DeletePhysicalDimensionAuthorization>();
            
            cltService.AddScoped(typeof(IPipelineBehavior<UpdatePhysicalDimensionCommand, IMessageResult<bool>>), typeof(MessageAuthorizationBehaviour<UpdatePhysicalDimensionCommand, bool>));
            cltService.TryAddTransient<IAuthorization<UpdatePhysicalDimensionCommand>, UpdatePhysicalDimensionAuthorization>();
			#endregion

			#region PhysicalDimension - Validation
			cltService.AddScoped(typeof(IPipelineBehavior<CreatePhysicalDimensionCommand, IMessageResult<Guid>>), typeof(MessageValidationBehaviour<CreatePhysicalDimensionCommand, Guid>));
			cltService.TryAddTransient<IValidation<CreatePhysicalDimensionCommand>, CreatePhysicalDimensionValidation>();

			cltService.AddScoped(typeof(IPipelineBehavior<DeletePhysicalDimensionCommand, IMessageResult<bool>>), typeof(MessageValidationBehaviour<DeletePhysicalDimensionCommand, bool>));
            cltService.TryAddTransient<IValidation<DeletePhysicalDimensionCommand>, DeletePhysicalDimensionValidation>();
            
            cltService.AddScoped(typeof(IPipelineBehavior<UpdatePhysicalDimensionCommand, IMessageResult<bool>>), typeof(MessageValidationBehaviour<UpdatePhysicalDimensionCommand, bool>));
            cltService.TryAddTransient<IValidation<UpdatePhysicalDimensionCommand>, UpdatePhysicalDimensionValidation>();
			#endregion

			#region TimePeriod - Authorization
			cltService.AddScoped(typeof(IPipelineBehavior<CreateTimePeriodCommand, IMessageResult<Guid>>), typeof(MessageAuthorizationBehaviour<CreateTimePeriodCommand, Guid>));
			cltService.TryAddTransient<IAuthorization<CreateTimePeriodCommand>, CreateTimePeriodAuthorization>();

			cltService.AddScoped(typeof(IPipelineBehavior<DeleteTimePeriodCommand, IMessageResult<bool>>), typeof(MessageAuthorizationBehaviour<DeleteTimePeriodCommand, bool>));
            cltService.TryAddTransient<IAuthorization<DeleteTimePeriodCommand>, DeleteTimePeriodAuthorization>();
            
            cltService.AddScoped(typeof(IPipelineBehavior<UpdateTimePeriodCommand, IMessageResult<bool>>), typeof(MessageAuthorizationBehaviour<UpdateTimePeriodCommand, bool>));
            cltService.TryAddTransient<IAuthorization<UpdateTimePeriodCommand>, UpdateTimePeriodAuthorization>();
			#endregion

			#region TimePeriod - Validation
			cltService.AddScoped(typeof(IPipelineBehavior<CreateTimePeriodCommand, IMessageResult<Guid>>), typeof(MessageValidationBehaviour<CreateTimePeriodCommand, Guid>));
			cltService.TryAddTransient<IValidation<CreateTimePeriodCommand>, CreateTimePeriodValidation>();

			cltService.AddScoped(typeof(IPipelineBehavior<DeleteTimePeriodCommand, IMessageResult<bool>>), typeof(MessageValidationBehaviour<DeleteTimePeriodCommand, bool>));
            cltService.TryAddTransient<IValidation<DeleteTimePeriodCommand>, DeleteTimePeriodValidation>();

            cltService.AddScoped(typeof(IPipelineBehavior<UpdateTimePeriodCommand, IMessageResult<bool>>), typeof(MessageValidationBehaviour<UpdateTimePeriodCommand, bool>));
            cltService.TryAddTransient<IValidation<UpdateTimePeriodCommand>, UpdateTimePeriodValidation>();
            #endregion

            return cltService;
        }
    }
}
