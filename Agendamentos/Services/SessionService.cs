using System.Security.Cryptography;
using Agendamentos.Database;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Agendamentos.Services;

public class SessionService(AgendamentosDbContext context, ILogger<SessionService> logger, IConfiguration configuration)
{

    public async Task<(string accessToken, string refreshToken)?> RefreshTokenAsync(string refreshToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var session = await GetSessionByRefreshTokenAsync(refreshToken);

            
            if (session is null || session.ExpiresAt < DateTime.UtcNow || session.Revoked )
            {
                throw new UnauthorizedAccessException("Refresh token inválido ou expirado.");
            }
            
            
            
            session.Revoked = true;
            context.Sessions.Update(session);
            await context.SaveChangesAsync();
            

            var displayName = session.User.Professor?.Nome ?? session.User.UserName;
        
            var newRefreshToken =  CreateSessionInContext(session.User, context);
            
            if (newRefreshToken is null)
            {
                throw new Exception("Não foi possível gerar um novo refresh token.");
            }
        
        
            var newAccessToken = UserService.GenerateAccessToken(displayName, session.User.Role, 
                session.User.Id, DateTime.UtcNow.AddMinutes(1), configuration);


            await context.SaveChangesAsync();

            await transaction.CommitAsync();
            
            
            return (newAccessToken, newRefreshToken);
        }
        catch (Exception e)
        {
            logger.LogError("Error refreshing token: {Message}", e.Message);    
            await transaction.RollbackAsync();
            return null;
        }
        
    } 
    
    
    private Session BuildSession(User user, string refreshToken)
    {
        return new Session
        {
            User = user,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(2),
            Deleted = false,
            Id = Guid.NewGuid(),
            Revoked = false,
            RefreshToken = refreshToken
        };
    } 
    
    public async Task<string?> SetSessionAsync(User user)
    {
        try
        {
            var refreshToken = GenerateRefreshToken();
            var session = BuildSession(user, refreshToken);
            context.Sessions.Add(session);
            await context.SaveChangesAsync();
            return refreshToken;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating session for user");
        }

        return null;
    }
    
    
    public string? CreateSessionInContext(User user, AgendamentosDbContext dbContext)
    {
        try
        {

            var refreshToken = GenerateRefreshToken();
            var session = BuildSession(user, refreshToken);

            dbContext.Sessions.Add(session);

            return refreshToken;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating session for user");
        }

        return null;
    }
    
    
    
    public async Task CleanExpiredSessionsAsync()
    {
        var expired = context.Sessions
            .Where(s => s.ExpiresAt < DateTime.UtcNow || s.Revoked || s.Deleted);

        context.Sessions.RemoveRange(expired);
        await context.SaveChangesAsync();
    }
    
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    
    
    public async Task<Session?> GetSessionByRefreshTokenAsync(string refreshToken)
    {
        return await context.Sessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken);
    }
    
    
}

