using Agendamentos.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Agendamentos.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var hostUrl = builder.Configuration["HOST_DEFAULT_URL"];
if (string.IsNullOrEmpty(hostUrl))
{
    throw new InvalidOperationException("Configuration 'HOST_DEFAULT_URL' is required but was not found.");
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AgendamentosDbContext>();
builder.Services.AddAppServices();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);


builder.Services.ConfigureSwagger();

builder.Services.ConfigureAuth(builder.Configuration);

var app = builder.Build();

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


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Logger.LogInformation("Starting application at {HostUrl}/swagger/index.html", hostUrl);

app.Run();