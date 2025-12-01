

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace WebAPI.Extensions
{
    public static class ExtensionAddServices
    {
        public static void AddCustomizeSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Protein Api Management", Version = "v1.0" });


                c.OperationFilter<ExtensionSwaggerFileOperationFilter>();


                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Protein Management for IT Company",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                    {securityScheme, new string[] { }}
            });
            });
        }
    }
}
        
    