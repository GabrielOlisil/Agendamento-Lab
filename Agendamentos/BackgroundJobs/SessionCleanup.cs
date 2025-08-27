using Agendamentos.Database;
using Agendamentos.Services;
using Quartz;

namespace Agendamentos.BackgroundJobs;

public class SessionCleanup(SessionService sessionService, ILogger<SessionCleanup> logger): IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Job {JobKey} started at {StartTime}", 
            context.JobDetail.Key, 
            DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-4)));
        
        await sessionService.CleanExpiredSessionsAsync();
        
        logger.LogInformation("Job {JobKey} finished at {EndTime}", 
            context.JobDetail.Key,
            DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-4)));
    }
}