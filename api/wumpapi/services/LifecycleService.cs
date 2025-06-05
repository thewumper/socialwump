using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using wumpapi.neo4j;
using wumpapi.Services;
using wumpapi.utils;

namespace wumpapi.services;

public class LifecycleService : ILifecycleService
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<LifecycleService> logger;
    private readonly IGameManager gameManager;
    private readonly IHostApplicationLifetime lifetime;
    private RepeatingVariableDelayExecutor autosaver = null!;

    public LifecycleService(
        IHostApplicationLifetime lifetimeService,
        ILogger<LifecycleService> logger,
        IGameManager gameManager,
        IServiceProvider serviceProvider)
    {
        this.logger = logger;
        this.gameManager = gameManager;
        this.serviceProvider = serviceProvider;
        lifetime = lifetimeService;
    }

    private async void Startup() 
    {
        try
        {
            logger.LogInformation("Starting game");
            await using var scope = serviceProvider.CreateAsyncScope();
            INeo4jDataAccess dataAccess = scope.ServiceProvider.GetRequiredService<INeo4jDataAccess>();
            IUserRepository userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            autosaver = new RepeatingVariableDelayExecutor(async Task<TimeSpan> () =>
            {
                await using var innerScope = serviceProvider.CreateAsyncScope();
                INeo4jDataAccess da = innerScope.ServiceProvider.GetRequiredService<INeo4jDataAccess>(); 
                IUserRepository ur = innerScope.ServiceProvider.GetRequiredService<IUserRepository>(); 
                gameManager.AutoSave(da, ur);
                return TimeSpan.FromMinutes(1);
            }, TimeSpan.FromSeconds(10), logger);

            await gameManager.Startup(dataAccess, userRepository);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Startup failed");
        }
    }

    private async Task Shutdown()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        INeo4jDataAccess dataAccess = scope.ServiceProvider.GetRequiredService<INeo4jDataAccess>();
        IUserRepository userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        autosaver?.Stop(); 
        gameManager.Shutdown(dataAccess, userRepository);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        lifetime.ApplicationStarted.Register(Startup);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Shutdown();
    }
}