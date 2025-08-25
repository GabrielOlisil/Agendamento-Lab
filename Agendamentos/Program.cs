using Agendamentos.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Agendamentos.DependencyInjection;
using Agendamentos.Middleware;

var builder = WebApplication.CreateBuilder(args);

var hostUrl = builder.Configuration["HOST_DEFAULT_URL"];
if (string.IsNullOrEmpty(hostUrl))
{
    throw new InvalidOperationException("Configuration 'HOST_DEFAULT_URL' is required but was not found.");
}

const string MyAllowSpecificOrigins = "frontentPolicy";

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
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("adminOnly", policy => policy.RequireRole("admin"));
    
    opt.AddPolicy("professorOnly", policy => policy.RequireRole("user"));
});
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
app.UseCors(MyAllowSpecificOrigins);


app.UseAuthentication();
app.UseAuthorization();

app.UseRequestLogging();

app.MapControllers();


app.Logger.LogInformation("Starting application at {HostUrl}/swagger/index.html", hostUrl);

app.Run();