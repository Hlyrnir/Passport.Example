#region WebServices
using Application.Authorization;
using Application.Common.Authentication;
using Application.Common.DataProtection;
using Application.ServiceCollectionExtension;
using Asp.Versioning;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Presentation;
using Presentation.Authorization.Policy;
using Presentation.Endpoint;
using Presentation.Endpoint.Authentication;
using Presentation.Endpoint.Authorization.Passport;
using Presentation.Endpoint.Authorization.PassportVisa;
using Presentation.Endpoint.PhysicalData;
using Presentation.Health;
using Presentation.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Security.Claims;
using System.Text;

WebApplicationBuilder webBuilder = WebApplication.CreateBuilder(args);

webBuilder.Services.AddOptions<DataProtectionSetting>()
	.Bind(webBuilder.Configuration.GetSection(DataProtectionSetting.SectionName))
	.ValidateOnStart();

webBuilder.Services.AddOptions<JwtTokenSetting>()
	.Bind(webBuilder.Configuration.GetSection(JwtTokenSetting.SectionName))
	.ValidateOnStart();

webBuilder.Services.AddOptions<PassportHashSetting>()
	.Bind(webBuilder.Configuration.GetSection(PassportHashSetting.SectionName))
	.ValidateOnStart();

// see Application -> ServiceCollectionExtension
webBuilder.Services.AddApplication(webBuilder.Configuration);
// see Infrastucture -> ServiceCollectionExtension
webBuilder.Services.AddInfrastructure(webBuilder.Configuration);
// see Presentation -> ServiceCollectionExtension
webBuilder.Services.AddPresentation();

webBuilder.Services.AddHealthChecks()
	.AddCheck<InfrastructureHealthCheck>(InfrastructureHealthCheck.Name);

// see https://code-maze.com/enabling-cors-in-asp-net-core/
webBuilder.Services.AddCors(oOption =>
{
	oOption.AddPolicy(name: "ClientPresentationLayerPolicy", plcyBuilder =>
	{
		plcyBuilder.WithOrigins(webBuilder.Configuration["JwtSetting:Issuer"])
				.AllowAnyHeader();
	});
});

webBuilder.Services.AddAuthentication(optAuthentication =>
{
	optAuthentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	optAuthentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	optAuthentication.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(optJwtBearer =>
{
	optJwtBearer.RequireHttpsMetadata = true;
	optJwtBearer.TokenValidationParameters = new TokenValidationParameters
	{
		ClockSkew = TimeSpan.FromSeconds(15),

		NameClaimType = ClaimTypes.NameIdentifier,
		ValidIssuer = webBuilder.Configuration["JwtSetting:Issuer"],
		ValidAudience = webBuilder.Configuration["JwtSetting:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(webBuilder.Configuration["JwtSetting:SecretKey"])),

		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true
	};
});

//// see https://learn.microsoft.com/en-us/aspnet/core/security/authorization/limitingidentitybyscheme?view=aspnetcore-7.0
webBuilder.Services.AddAuthorization(optAuthorization =>
{
	optAuthorization.FallbackPolicy = new AuthorizationPolicyBuilder()
	.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
	.RequireAuthenticatedUser()
	.Build();

	optAuthorization.AddPolicy(EndpointAuthorization.Passport, EndpointPolicy.EndpointWithPassport());
});

webBuilder.Services.AddApiVersioning(optVersion =>
{
	optVersion.DefaultApiVersion = new ApiVersion(1.0);
	optVersion.AssumeDefaultVersionWhenUnspecified = true;
	optVersion.ReportApiVersions = true;
	optVersion.ApiVersionReader = new MediaTypeApiVersionReader("api-version");
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
webBuilder.Services.AddEndpointsApiExplorer();

webBuilder.Services.AddSwaggerGen();
webBuilder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOption>();
#endregion

#region WebApplication
WebApplication webApplication = webBuilder.Build();

webApplication.AddEndpointVersionSet();

// Configure the HTTP request pipeline.
if (webApplication.Environment.IsEnvironment("Development"))
{
	webApplication.UseSwagger();
	webApplication.UseSwaggerUI(optSwagger=>
	{
		optSwagger.DocumentTitle = "DEVELOPMENT - API of CQRS_Prototype";
	});
}
else if(webApplication.Environment.IsEnvironment("Testing"))
{
	webApplication.UseSwagger();
	webApplication.UseSwaggerUI(optSwagger =>
	{
		optSwagger.DocumentTitle = "TESTING - API of CQRS_Prototype";
	});
}
else // manually added
{
	webApplication.UseExceptionHandler("/error");
	webApplication.UseHsts();
}

webApplication.MapHealthChecks("_health")
	.AllowAnonymous();

webApplication.UseHttpsRedirection();

//// see https://code-maze.com/enabling-cors-in-asp-net-core/#creating-applications
//webApplication.UseCors("ClientPresentationLayerPolicy");

// Use JWT bearer token for authentication
webApplication.UseAuthentication();
// Authorize only authenticated user to endpoints
webApplication.UseAuthorization();

webApplication.AddGenerateTokenByCredentialEndpoint();
webApplication.AddGenerateTokenByRefreshTokenEndpoint();

webApplication.AddFindPassportByIdEndpoint();
webApplication.AddRegisterPassportEndpoint();
webApplication.AddUpdatePassportEndpoint();

webApplication.AddFindPassportVisaByIdEndpoint();
webApplication.AddFindPassportVisaByPassportIdEndpoint();
webApplication.AddCreatePassportVisaEndpoint();
webApplication.AddUpdatePassportVisaEndpoint();
webApplication.AddDeletePassportVisaEndpoint();

webApplication.AddFindPhysicalDimensionByFilterEndpoint();
webApplication.AddFindPhysicalDimensionByIdEndpoint();
webApplication.AddCreatePhysicalDimensionEndpoint();
webApplication.AddUpdatePhysicalDimensionEndpoint();
webApplication.AddDeletePhysicalDimensionEndpoint();

webApplication.AddFindTimePeriodByFilterEndpoint();
webApplication.AddFindTimePeriodByIdEndpoint();
webApplication.AddCreateTimePeriodEndpoint();
webApplication.AddUpdateTimePeriodEndpoint();
webApplication.AddDeleteTimePeriodEndpoint();

webApplication.Run();
#endregion