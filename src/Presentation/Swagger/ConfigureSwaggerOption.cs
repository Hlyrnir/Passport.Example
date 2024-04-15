using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Presentation.Swagger
{
	public class ConfigureSwaggerOption : IConfigureOptions<SwaggerGenOptions>
	{
		public void Configure(SwaggerGenOptions swaggerOption)
		{
			swaggerOption.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Scheme = "Bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header,
				Name = "Authorization",
				Description = "Bearer Authentication with JWT token",
				Type = SecuritySchemeType.Http
			});
			swaggerOption.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Id = "Bearer",
							Type = ReferenceType.SecurityScheme
						}
					},
					new List<string>()
				}
			});
		}
	}
}
