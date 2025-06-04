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
        
        lifetimeService.ApplicationStarted.Register(Startup);
        lifetimeService.ApplicationStopped.Register(Shutdown);
    }

    private async void Startup() 
    {
        try
        {
            logger.LogInformation("Starting game");
            using var scope = serviceProvider.CreateScope();
            INeo4jDataAccess dataAccess = scope.ServiceProvider.GetRequiredService<INeo4jDataAccess>();
            IUserRepository userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            autosaver = new RepeatingVariableDelayExecutor(() => 
            {
                using var innerScope = serviceProvider.CreateScope();
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

    private void Shutdown()
    {
        using var scope = serviceProvider.CreateScope();
        INeo4jDataAccess dataAccess = scope.ServiceProvider.GetRequiredService<INeo4jDataAccess>();
        IUserRepository userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        autosaver?.Stop(); 
        gameManager.Shutdown(dataAccess, userRepository);
    }
}