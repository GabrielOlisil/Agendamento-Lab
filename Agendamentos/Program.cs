using Agendamentos.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Agendamentos.DependencyInjection;
using Agendamentos.Middleware;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDataProtection().UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
{
    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
    ValidationAlgorithm = ValidationAlgorithm.HMACSHA512
});

var hostUrl = builder.Configuration["HOST_DEFAULT_URL"];
if (string.IsNullOrEmpty(hostUrl))
{
    throw new InvalidOperationException("Configuration 'HOST_DEFAULT_URL' is required");
}

const string myAllowSpecificOrigins = "frontentPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "frontentPolicy",
        policy  =>
        {
            policy.WithOrigins("*")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AgendamentosDbContext>();
builder.Services.AddAppServices();


builder.Services.AddAuthentication();
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("adminOnly", policy => policy.RequireRole("admin"))
    .AddPolicy("professorOnly", policy => policy.RequireRole("user"));




builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);


builder.Services.ConfigureSwagger();

builder.Services.ConfigureAuth(builder.Configuration);


var app = builder.Build();


app.Services.ApplyMigrations();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    

    app.UseExceptionHandler("/error");

}
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error/prod");
}
app.UseCors(myAllowSpecificOrigins);


app.UseAuthentication();
app.UseAuthorization();

app.UseRequestLogging();

app.MapControllers();


app.Logger.LogInformation("Starting application at {HostUrl}/swagger/index.html", hostUrl);

app.Run();