using Agendamentos.BackgroundJobs;
using Agendamentos.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Agendamentos.DependencyInjection;
using Agendamentos.Helpers;
using Agendamentos.Middleware;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Quartz;

var builder = WebApplication.CreateBuilder(args);


if (ContainerHelpers.TryGetAppDirectoryFolder("/App/DataProtectionKeys", out var directory))
{
    builder.Services.AddDataProtection().UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
    {
        EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
        ValidationAlgorithm = ValidationAlgorithm.HMACSHA512
    }).PersistKeysToFileSystem(directory);
}





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

builder.Services.AddQuartz(quartz =>
{
    var jobKey = new JobKey("SessionCleanup");
    quartz.AddJob<SessionCleanup>(opts => opts.WithIdentity(jobKey));

    quartz.AddTrigger(opts =>
    {
        opts.ForJob(jobKey)
            .WithIdentity("SessionCleanup-trigger")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInHours(1)
                .RepeatForever());
    });

});
builder.Services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);


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