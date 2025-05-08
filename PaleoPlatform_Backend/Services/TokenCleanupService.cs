using PaleoPlatform_Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace PaleoPlatform_Backend.Services
{
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<TokenCleanupService> _logger;

        public TokenCleanupService(IServiceProvider services, ILogger<TokenCleanupService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var cutoff = DateTime.UtcNow.AddDays(-1);
                    var expired = await dbContext.ExpiredTokens
                        .Where(t => t.ExpirationDate < cutoff)
                        .ToListAsync(stoppingToken);

                    dbContext.ExpiredTokens.RemoveRange(expired);
                    await dbContext.SaveChangesAsync(stoppingToken);

                    _logger.LogInformation("Cleaned up {Count} expired tokens", expired.Count);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Error cleaning tokens");
                }

                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
            }
        }
    }
 }
