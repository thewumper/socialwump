using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using wumpapi.configuration;
using wumpapi.game;
using wumpapi.game.Items;
using wumpapi.game.Items.interfaces;
using wumpapi.neo4j;
using wumpapi.Services;
using wumpapi.structures;
using SessionExpiredException = Neo4j.Driver.SessionExpiredException;

namespace wumpapi.api;

public class Webapp
{
    private ApplicationSettings settings;
    private WebApplication app;
    public Webapp(string[] args)
    {
        WebApplicationBuilder builder = CreateBuilder(args);
        AddServices(builder);
        app = builder.Build();
        RegisterObjects();
        SetupApi();
        StartGame();
    }




    private WebApplicationBuilder CreateBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args); 
        builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
        settings = new ApplicationSettings();
        builder.Configuration.GetSection("ApplicationSettings").Bind(settings);
        return builder;
    }

    private void AddServices(WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        builder.Services.AddSingleton(GraphDatabase.Driver(settings.Neo4jConnection, AuthTokens.Basic(settings.Neo4jUser, settings.Neo4jPassword)));
        builder.Services.AddScoped<INeo4jDataAccess, Neo4jDataAccess>();
        builder.Services.AddTransient<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        builder.Services.AddSingleton<ISessionManager, SessionManager>();
        builder.Services.AddSingleton<IPlayerStats, PlayerStats>();
        builder.Services.AddSingleton<IGameManager, GameManager>();
        builder.Services.AddSingleton<IItemRegistry, ItemRegistry>();
    }
    
    private void RegisterObjects()
    {
        ItemRegisterer.RegisterItems(app.Services.GetService<IItemRegistry>()!);
        // do more registration here
    }
    
    private void SetupApi()
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        app.UseHttpsRedirection();
        RegisterEndpoints();
    }
    private void StartGame()
    {
        Task.Run(async () =>
        {
            var scope = app.Services.CreateAsyncScope();
            try
            {
                var service = scope.ServiceProvider;
                await app.Services.GetService<IGameManager>()!.Startup(service.GetService<INeo4jDataAccess>()!,
                    service.GetRequiredService<IUserRepository>()!, service.GetRequiredService<IItemRegistry>()!);
            }
            finally
            {
                await scope.DisposeAsync();
            }
        });
    }

    private void RegisterEndpoints()
    {
        
        app.MapPost("/login", LoginHandler).WithName("login").Produces<LoginResponse>().Produces<ErrorResponse>(StatusCodes.Status401Unauthorized);
        app.MapPost("/logout", LogoutHandler).WithName("logout").Produces<Ok>().Produces<ErrorResponse>(StatusCodes.Status401Unauthorized);
        // TODO: add all the proper codes
        app.MapGet("/graph", GraphHandler).WithName("graph");
        app.MapGet("/users", async (IUserRepository userRepository) => await userRepository.GetUsers())
            .WithName("users");
        app.MapPost("/createaccount", CreateAccountHandler).WithName("CreateAccount").Produces(StatusCodes.Status201Created).Produces<ErrorResponse>(StatusCodes.Status409Conflict);
        // TODO: Testing purposes only! Delete this soon
        app.MapPost("/createRelationship", CreateRelationshipHandler).WithName("CreateRelationship");
        app.MapGet("/dropdatabase", DropDatabase).WithName("DropDatabase");
        app.MapGet("/getLeaderboard", GetLeaderboardHandler).WithName("GetLeaderboard");
        app.MapPost("/validateauth", ValidateAuthHandler).WithName("ValidateAuth");
        app.MapGet("/iteminfo", ItemInfoHandler).WithName("ItemInfo");
        app.MapGet("/gamestate", GameStateHandler).WithName("GameState");
        app.MapGet("/playersingame", PlayersInGameHandler).WithName("PlayersInGame");
        app.MapPost("/joingame", PlayerJoinHandler).WithName("Joingame");
        app.MapPost("/createalliance", CreateAllianceHandler).WithName("CreateAlliance");
        app.MapPost("/joinalliance", JoinAllianceHandler).WithName("JoinAlliance");
        app.MapPost("/leavealliance", LeaveAllianceHandler).WithName("LeaveAlliance");
        app.MapPost("/useability", UseAbilityHandler).WithName("UseAbility");

    }

    private async Task<IResult> UseAbilityHandler(ISessionManager sessionManager,IUserRepository userRepository ,[FromServices] IGameManager gameManger, [FromBody] AbilityRequest request)
    {
        if (sessionManager.IsSessionValid(request.SessionToken))
        {
            Game? currentGame = gameManger.GetCurrentGame();
            if (currentGame != null)
            {
                User user = sessionManager.GetAuthedUser(request.SessionToken);
                Player? player = currentGame.GetPlayer(user);
                if (player != null)
                {
                    IItem? itemToUse = player.Items[request.ItemSlot];
                    User target;
                    try
                    {
                        target = await userRepository.GetUser(request.Target);
                    }
                    catch (UserNotFoundException e)
                    {
                        return Results.BadRequest("Invalid target");
                    }

                    if (itemToUse == null)
                    {
                        return Results.BadRequest("You dont have an item in that slot");
                    }
                    if (target.Username == user.Username)
                    {
                        if (itemToUse is IUsableItem usableItem)
                        {
                            if (usableItem.Use(player))
                            {
                                return Results.Ok();
                            }
                            else
                            {
                                return Results.BadRequest("Item failed to use, maybe it's on cooldown");
                            }
                        }
                    }
                    else
                    {
                        if (itemToUse is ITargetableItem targetableItem)
                        {
                            Player? targetPlayer = currentGame.GetPlayer(target);
                            if (targetPlayer != null)
                            {
                                if (targetableItem.Use(player, targetPlayer))
                                {
                                    return Results.Ok();
                                }
                                else
                                {
                                    return Results.BadRequest("Item failed to use, maybe it's on cooldown");
                                }
                            }
                            else
                            {
                                return Results.BadRequest("Target player doesn't exist");
                            }
                        }
                    }
                    
                    return Results.Ok();
                }
                else
                {
                    return Results.BadRequest(new ErrorResponse("Player is not in the game"));
                }
            }
            else
            {
                return Results.BadRequest(new ErrorResponse("Game not started"));
            }
        }
        else
        {
            return Results.BadRequest(new ErrorResponse("Invalid Session"));
        }
    }


    private IResult PlayerJoinHandler(ISessionManager sessionManager, [FromServices] IGameManager gameManager, [FromBody] PlayerJoinRequest request)
    {
        if (sessionManager.IsSessionValid(request.SessionToken))
        {
            User user = sessionManager.GetAuthedUser(request.SessionToken);
            Player player = gameManager.AddPlayer(user);

            return Results.Ok(new PlayerJoinResponse(player, user));
        }
        else
        {
            return Results.BadRequest(new ErrorResponse("Invalid Session"));
        }
        
    }
    
    private IResult CreateAllianceHandler(ISessionManager sessionManager, [FromServices] IGameManager gameManager, [FromBody] AllianceRequest request)
    {
        if (sessionManager.IsSessionValid(request.SessionToken))
        {
            User user = sessionManager.GetAuthedUser(request.SessionToken);
            
            try
            {
                Player? player = gameManager.GetActiveGame().GetPlayer(user);

                if (player == null)
                {
                    return Results.BadRequest(new ErrorResponse("Player has not joined yet!"));
                }
                
                return Results.Ok(new AllianceResponse(gameManager.GetActiveGame().CreateAlliance(request.AllianceName, player)!));
            }
            catch (InvalidOperationException e)
            {
                return Results.BadRequest(new ErrorResponse(e.Message));
            }
        }
        else
        {
            return Results.BadRequest(new ErrorResponse("Invalid Session"));
        }
    }
    
    private IResult JoinAllianceHandler(ISessionManager sessionManager, [FromServices] IGameManager gameManager, [FromBody] AllianceRequest request)
    {
        if (sessionManager.IsSessionValid(request.SessionToken))
        {
            User user = sessionManager.GetAuthedUser(request.SessionToken);

            try
            {
                Player? player = gameManager.GetActiveGame().GetPlayer(user);

                if (player == null)
                {
                    return Results.BadRequest(new ErrorResponse("Player has not joined yet!"));
                }
                
                return Results.Ok(new AllianceResponse(gameManager.GetActiveGame().JoinAlliance(request.AllianceName, player)!));
            }
            catch (InvalidOperationException e)
            {
                return Results.BadRequest(new ErrorResponse(e.Message));
            }
        }
        else
        {
            return Results.BadRequest(new ErrorResponse("Invalid Session"));
        }
    }
    
    private IResult LeaveAllianceHandler(ISessionManager sessionManager, [FromServices] IGameManager gameManager, [FromBody] AllianceLeaveRequest request)
    {
        if (sessionManager.IsSessionValid(request.SessionToken))
        {
            User user = sessionManager.GetAuthedUser(request.SessionToken);

            try
            {
                Player? player = gameManager.GetActiveGame().GetPlayer(user);

                if (player == null)
                {
                    return Results.BadRequest(new ErrorResponse("Player has not joined yet!"));
                }

                gameManager.GetActiveGame().LeaveAlliance(player);
                return Results.Ok();
            }
            catch (InvalidOperationException e)
            {
                return Results.BadRequest(new ErrorResponse(e.Message));
            }
        }
        else
        {
            return Results.BadRequest(new ErrorResponse("Invalid Session"));
        }
    }


    private IResult PlayersInGameHandler([FromServices] IGameManager gameManager)
    {
        return Results.Ok(new PlayerCountResponse(gameManager.GetCurrentGame()!.WaitingPlayers(), Constants.MinimumPlayers));
    }
    
    private IResult GameStateHandler([FromServices] IGameManager gameManager)
    {
        string result;
        switch (gameManager.GetGameState())
        {
            case GameState.Active:
                result = "Active";
                break;
            case GameState.Waiting:
                result = "Waiting";
                break;
            case GameState.Starting:
                result = "Starting";
                break;
            case null:
            default:
                result = "Unknown";
                break;
        }
        return Results.Ok(result);
    }

    private IResult ItemInfoHandler(IItemRegistry itemRegistry)
    {
        return Results.Ok(itemRegistry.GetItems());
    }

    private IResult ValidateAuthHandler(ISessionManager sessionManager, [FromBody] ValidateAuthRequest request)
    {
        if (request.SessionToken == null)
        {
            return Results.BadRequest("Session token cannot be null");
        }
        return Results.Ok(sessionManager.IsSessionValid(request.SessionToken) ? new ValidateAuthResponse(true, sessionManager.GetAuthedUser(request.SessionToken)) : new ValidateAuthResponse(false, null));
    }

    private Task DropDatabase(INeo4jDataAccess neo4JDataAccess)
    {
        neo4JDataAccess.ExecuteWriteTransactionAsync<bool>(@"MATCH (n) DETACH DELETE n RETURN true");
        return Task.CompletedTask;
    }

    private IResult GetLeaderboardHandler(IPlayerStats playerStats, [FromBody] GetLeaderboardRequest request)
    {
        return Enum.TryParse(request.Category, out PlayerStatTypes playerStatType) ? Results.Ok(playerStats.GetLeaderboard(playerStatType)) : Results.BadRequest($"Invalid category, categories are: {string.Join(", ", Enum.GetNames(typeof(PlayerStatTypes)))}");
    }

    public void Start()
    {
        app.Run();
    }

    private async Task<IResult> CreateRelationshipHandler(IUserRepository userRepository, ISessionManager sessionManager, [FromBody] CreateRelationshipRequest request)
    {
        try
        {
            User initiator = sessionManager.GetAuthedUser(request.SessionToken);
            User target = await userRepository.GetUser(username: request.TargetUser);
            bool succesful = await userRepository.CreateRelationship(initiator, target, request.RelationshipName, request.Data);
            if (succesful)
            {
                Connection result = new Connection(initiator, target, request.RelationshipName, request.Data);
                return Results.Ok(new RelationshipCreatedResponse(result));
            }
            else
            {
                return Results.BadRequest(new ErrorResponse("Failed to create relationship"));
            }
        }
        catch (InvalidSessionException)
        {
            return Results.BadRequest(new ErrorResponse("Invalid session"));
        }
        catch (SessionExpiredException)
        {
            return Results.BadRequest(new ErrorResponse("Session expired, please login again"));
        }
        catch (UserNotFoundException)
        {
            return Results.BadRequest(new ErrorResponse("The target user could not be found"));
        }
    }

    private async Task<IResult> CreateAccountHandler(IUserRepository userRepo, IPasswordHasher<User> passwordHasher, [FromBody] CreateUserRequest request)
    {
        // Check for existing user
        if (await userRepo.UserExists(request.Username, request.Email))
        {
            return Results.Conflict(new ErrorResponse("A user with the given username already exists"));
        }

        if (!Regex.IsMatch(request.Username, @"^[\w]+$"))
            return Results.BadRequest(
                new ErrorResponse("Invalid username, must contain only numbers letters and underscore"));
        User user = new User(request.Username, request.Email, request.FirstName, request.LastName);
        user.Password = passwordHasher.HashPassword(user, request.Password);
        bool successful = await userRepo.AddUser(user);
        if (!successful)
        {
            throw new Exception("Failed to create user");
        }

        return Results.Created();
    }

    private static async Task<IResult> LoginHandler(ISessionManager sessionManager, IUserRepository userRepository, IPasswordHasher<User> passwordHasher, [FromBody] LoginUserRequest loginInfo)
    {
        try
        {
            Tuple<string, User> sessiondata = await sessionManager.AuthUser(loginInfo.Username, loginInfo.Password, userRepository, passwordHasher);
            return Results.Ok(new LoginResponse(sessiondata.Item1, sessiondata.Item2));
        }
        catch (UserNotFoundException)
        {
            return Results.BadRequest(new ErrorResponse("Invalid username or password"));
        }
        catch (IncorrectPasswordException)
        {
            return Results.BadRequest(new ErrorResponse("Invalid username or password"));
        }
    }
    private IResult LogoutHandler(ISessionManager sessionManager, [FromBody] LogoutRequest logoutRequest)
    {
        try
        {
            sessionManager.Logout(logoutRequest.SessionToken);
            return Results.Ok();
        }
        catch (InvalidSessionException)
        {
            return Results.BadRequest(new ErrorResponse("Invalid session"));
        }
    }
    
    private IResult GraphHandler([FromServices] IGameManager gameManager)
    {
        if (gameManager.GetGameState() == GameState.Active)
        {
            return Results.Ok(gameManager.GetActiveGame().Graph());
        }
        else
        {
            return Results.NoContent();
        }
    }
    
}



