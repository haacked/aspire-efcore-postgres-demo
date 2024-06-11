using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace AspireDemo.Web.Startup;

public class DemoDbInitializer(IServiceProvider serviceProvider, ILogger<DemoDbInitializer> logger)
    : BackgroundService
{
    public const string ActivitySourceName = "Migrations";

    private readonly ActivitySource _activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DemoDbContext>();

        await InitializeDatabaseAsync(dbContext, cancellationToken);
    }

    async Task InitializeDatabaseAsync(DemoDbContext dbContext, CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity("Initializing demo database", ActivityKind.Client);

        var sw = Stopwatch.StartNew();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(dbContext.Database.MigrateAsync, cancellationToken);

        await SeedAsync(dbContext, cancellationToken);

        logger.LogInformation("Database initialization completed after {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
    }

    async Task SeedAsync(DemoDbContext dbContext, CancellationToken cancellationToken)
    {
        logger.LogInformation("Seeding database");

        static List<User> GetPreconfiguredUsers()
        {
            return [
                new() { Name = "Haacked" },
            ];
        }

        if (!dbContext.Users.Any())
        {
            dbContext.Users.AddRange(GetPreconfiguredUsers());
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
