using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TodoApi.StartupConfig;
using TodoLibrary.Data;
using TodoLibrary.DataAccess;

var builder = WebApplication.CreateBuilder(args);

DependencyInjectionExtensions.AddStandardServices(builder);
DependencyInjectionExtensions.AddCustomServices(builder);

builder.Services.AddAuthorization(opts =>
{
    opts.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

string? secretKey = builder.Configuration.GetValue<string>("Authentication:SecretKey");
if (secretKey is null)
{
    throw new Exception("Secret key is missing");
}
else
{
    builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration.GetValue<string>("Authentication:Issuer"),
            ValidAudience = builder.Configuration.GetValue<string>("Authentication:Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))
        };
    });
}

var connectionString = builder.Configuration.GetConnectionString("Default");
if(connectionString is null)
{
    throw new Exception("Connection string is missing");
}
else
{
    builder.Services.AddHealthChecks()
        .AddSqlServer(connectionString);
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opts =>
    {
        opts.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        opts.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapHealthChecks("/health").AllowAnonymous();

app.Run();
