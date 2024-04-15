using Application.Common.Authorization;
using Application.Common.Validation;
using Application.Interface.Authorization;
using Application.Interface.Result;
using Application.Interface.Validation;
using Application.Query.Authorization.Passport.ById;
using Application.Query.Authorization.PassportHolder.ById;
using Application.Query.Authorization.PassportVisa.ById;
using Application.Query.Authorization.PassportVisa.ByPassport;
using Application.Query.PhysicalData.PhysicalDimension.ByFilter;
using Application.Query.PhysicalData.PhysicalDimension.ById;
using Application.Query.PhysicalData.TimePeriod.ByFilter;
using Application.Query.PhysicalData.TimePeriod.ById;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Application.ServiceCollectionExtension
{
	public static class QueryBehaviorExtension
    {
        public static IServiceCollection AddQueryBehaviour(this IServiceCollection cltService)
        {
			#region Passport - Authorization
			cltService.AddScoped(typeof(IPipelineBehavior<PassportByIdQuery, IMessageResult<PassportByIdResult>>), typeof(MessageAuthorizationBehaviour<PassportByIdQuery, PassportByIdResult>));
			cltService.TryAddTransient<IAuthorization<PassportByIdQuery>, PassportByIdAuthorization>();

			cltService.AddScoped(typeof(IPipelineBehavior<PassportHolderByIdQuery, IMessageResult<PassportHolderByIdResult>>), typeof(MessageAuthorizationBehaviour<PassportHolderByIdQuery, PassportHolderByIdResult>));
			cltService.TryAddTransient<IAuthorization<PassportHolderByIdQuery>, PassportHolderByIdAuthorization>();

			cltService.AddScoped(typeof(IPipelineBehavior<PassportVisaByIdQuery, IMessageResult<PassportVisaByIdResult>>), typeof(MessageAuthorizationBehaviour<PassportVisaByIdQuery, PassportVisaByIdResult>));
			cltService.TryAddTransient<IAuthorization<PassportVisaByIdQuery>, PassportVisaByIdAuthorization>();

			cltService.AddScoped(typeof(IPipelineBehavior<PassportVisaByPassportIdQuery, IMessageResult<PassportVisaByPassportIdResult>>), typeof(MessageAuthorizationBehaviour<PassportVisaByPassportIdQuery, PassportVisaByPassportIdResult>));
			cltService.TryAddTransient<IAuthorization<PassportVisaByPassportIdQuery>, PassportVisaByPassportIdAuthorization>();
			#endregion

			#region Passport - Validation
			cltService.AddScoped(typeof(IPipelineBehavior<PassportByIdQuery, IMessageResult<PassportByIdResult>>), typeof(MessageValidationBehaviour<PassportByIdQuery, PassportByIdResult>));
			cltService.TryAddTransient<IValidation<PassportByIdQuery>, PassportByIdValidation>();

			cltService.AddScoped(typeof(IPipelineBehavior<PassportHolderByIdQuery, IMessageResult<PassportHolderByIdResult>>), typeof(MessageValidationBehaviour<PassportHolderByIdQuery, PassportHolderByIdResult>));
			cltService.TryAddTransient<IValidation<PassportHolderByIdQuery>, PassportHolderByIdValidation>();

			cltService.AddScoped(typeof(IPipelineBehavior<PassportVisaByIdQuery, IMessageResult<PassportVisaByIdResult>>), typeof(MessageValidationBehaviour<PassportVisaByIdQuery, PassportVisaByIdResult>));
			cltService.TryAddTransient<IValidation<PassportVisaByIdQuery>, PassportVisaByIdValidation>();

			cltService.AddScoped(typeof(IPipelineBehavior<PassportVisaByPassportIdQuery, IMessageResult<PassportVisaByPassportIdResult>>), typeof(MessageValidationBehaviour<PassportVisaByPassportIdQuery, PassportVisaByPassportIdResult>));
			cltService.TryAddTransient<IValidation<PassportVisaByPassportIdQuery>, PassportVisaByPassportIdValidation>();
			#endregion

			#region PhysicalDimension - Authorization
			cltService.AddScoped(typeof(IPipelineBehavior<PhysicalDimensionByIdQuery, IMessageResult<PhysicalDimensionByIdResult>>), typeof(MessageAuthorizationBehaviour<PhysicalDimensionByIdQuery, PhysicalDimensionByIdResult>));
            cltService.TryAddTransient<IAuthorization<PhysicalDimensionByIdQuery>, PhysicalDimensionByIdAuthorization>();

            cltService.AddScoped(typeof(IPipelineBehavior<PhysicalDimensionByFilterQuery, IMessageResult<PhysicalDimensionByFilterResult>>), typeof(MessageAuthorizationBehaviour<PhysicalDimensionByFilterQuery, PhysicalDimensionByFilterResult>));
            cltService.TryAddTransient<IAuthorization<PhysicalDimensionByFilterQuery>, PhysicalDimensionByFilterAuthorization>();
			#endregion

			#region PhysicalDimension - Validation
			cltService.AddScoped(typeof(IPipelineBehavior<PhysicalDimensionByFilterQuery, IMessageResult<PhysicalDimensionByFilterResult>>), typeof(MessageValidationBehaviour<PhysicalDimensionByFilterQuery, PhysicalDimensionByFilterResult>));
			cltService.TryAddTransient<IValidation<PhysicalDimensionByFilterQuery>, PhysicalDimensionByFilterValidation>();

			cltService.AddScoped(typeof(IPipelineBehavior<PhysicalDimensionByIdQuery, IMessageResult<PhysicalDimensionByIdResult>>), typeof(MessageValidationBehaviour<PhysicalDimensionByIdQuery, PhysicalDimensionByIdResult>));
			cltService.TryAddTransient<IValidation<PhysicalDimensionByIdQuery>, PhysicalDimensionByIdValidation>();
			#endregion

			#region TimePeriod - Authorization
			cltService.AddScoped(typeof(IPipelineBehavior<TimePeriodByIdQuery, IMessageResult<TimePeriodByIdResult>>), typeof(MessageAuthorizationBehaviour<TimePeriodByIdQuery, TimePeriodByIdResult>));
            cltService.TryAddTransient<IAuthorization<TimePeriodByIdQuery>, TimePeriodByIdAuthorization>();

            cltService.AddScoped(typeof(IPipelineBehavior<TimePeriodByFilterQuery, IMessageResult<TimePeriodByFilterResult>>), typeof(MessageAuthorizationBehaviour<TimePeriodByFilterQuery, TimePeriodByFilterResult>));
            cltService.TryAddTransient<IAuthorization<TimePeriodByFilterQuery>, TimePeriodByFilterAuthorization>();
			#endregion

			#region TimePeriod - Validation
			cltService.AddScoped(typeof(IPipelineBehavior<TimePeriodByFilterQuery, IMessageResult<TimePeriodByFilterResult>>), typeof(MessageValidationBehaviour<TimePeriodByFilterQuery, TimePeriodByFilterResult>));
			cltService.TryAddTransient<IValidation<TimePeriodByFilterQuery>, TimePeriodByFilterValidation>();

			cltService.AddScoped(typeof(IPipelineBehavior<TimePeriodByIdQuery, IMessageResult<TimePeriodByIdResult>>), typeof(MessageValidationBehaviour<TimePeriodByIdQuery, TimePeriodByIdResult>));
			cltService.TryAddTransient<IValidation<TimePeriodByIdQuery>, TimePeriodByIdValidation>();
			#endregion

			return cltService;
        }
    }
}
