using Autofac;
using Autofac.Extensions.DependencyInjection;
using Business.DependencyResolver.Autofac;
using Core.DependencyResolvers;
using Core.Extensions;
using Core.Utilities.Security.Encryption;
using Core.Utilities.Security.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WebAPI.StartupExtensions;
using DataAccess.Concrete.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
IdentityModelEventSource.ShowPII = true;// jwt hatası için , kaldırılacak

var builder = WebApplication.CreateBuilder(args);

// Autofac DI
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new AutofacBusinessModule());
});

// Controllers
builder.Services.AddControllers();

// JWT
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = tokenOptions.Issuer,
            ValidAudience = tokenOptions.Audience,
            IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("[JWT ERROR] " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine("[JWT CHALLENGE] " + context.ErrorDescription);
                return Task.CompletedTask;
            }
        };
    });

// Core module & dependency resolvers
builder.Services.AddDependencyResolvers(new ICoreModule[]
{
    new CoreModule()
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlite(connectionString);
});
// Message broker options (RabbitMQ)
//var messageBrokerSection = builder.Configuration.GetSection("MessageBrokerOptions");
//var messageBrokerOptions = messageBrokerSection.Get<MessageBrokerOptions>();
//builder.Services.Configure<MessageBrokerOptions>(messageBrokerSection);
//if (messageBrokerOptions != null && messageBrokerOptions.Enabled)
//{
//   builder.Services.AddHostedService<MqConsumerHelper>();
//}

// Email configuration
//builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));

// Swagger + JWT

builder.Services.AddCustomizeSwagger();

// CORS servisi
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .WithOrigins("http://localhost:5173") //Sadece Dev Ortamında herşeye izin verdik
            .AllowAnyMethod()                     
            .AllowAnyHeader()                     
            .AllowCredentials());                 
});

var app = builder.Build();
await app.InitializeDatabaseAsync();//başlangıç için migration 

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI v1");
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
