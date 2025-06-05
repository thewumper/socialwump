using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using wumpapi.configuration;
using wumpapi.game;
using wumpapi.game.events;
using wumpapi.game.Items;
using wumpapi.game.Items.genericitems;
using wumpapi.game.Items.interfaces;
using wumpapi.neo4j;
using wumpapi.services;
using wumpapi.Services;
using wumpapi.structures;
using wumpapi.utils;
using SessionExpiredException = Neo4j.Driver.SessionExpiredException;

namespace wumpapi.api;
/// <summary>
/// Manages API endpoints and services
/// </summary>
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
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
        builder.Services.AddSingleton<ISessionManager, SessionManager>();
        builder.Services.AddSingleton<IPlayerStats, PlayerStats>();
        builder.Services.AddSingleton<IGameManager, GameManager>();
        builder.Services.AddSingleton<IItemRegistry, ItemRegistry>();
        builder.Services.AddSingleton<IEventManager, EventManager>();
        builder.Services.AddSingleton<ILifecycleService, LifecycleService>();
        builder.Services.AddHostedService(provider => 
            provider.GetRequiredService<ILifecycleService>() as LifecycleService);
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
        app.MapPost("/purchasefromshop", ShopPurchaseHandler).WithName("PurchaseFromShop");
        app.MapPost("/sellitem", SellItemHandler).WithName("SellItemHandler");
        app.MapPost("/getplayer", GetPlayerHandler).WithName("GetPlayer");
        app.MapPost("/sharepower", PowerShareHandler).WithName("SharePower");
        app.MapPost("/shareitem", ItemShareHandler).WithName("ShareItem");
        app.MapPost("/events", EventsHandler).WithName("events");
        
    }

    private IResult EventsHandler(IEventManager eventManager,[FromBody] EventRequest request)
    {
        return Results.Ok(eventManager.GetEvents(request.LastEvent).Select(r => (object)r).ToList());
    }

    private async Task<IResult> ItemShareHandler(ISessionManager sessionManager, IUserRepository userRepository, [FromServices] IGameManager gameManger, IEventManager events, [FromBody] ItemShareRequest request)
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
                    User target;
                    try
                    {
                        target = await userRepository.GetUser(request.Username);
                    }
                    catch (UserNotFoundException e)
                    {
                        return Results.BadRequest("Invalid target");
                    }

                    if (target.Username == user.Username)
                    {
                        return Results.BadRequest("You cannot give power to yourself!");
                    }

                    Player? targetPlayer = currentGame.GetPlayer(target);
                    if (targetPlayer != null)
                    {
                        if (request.Slot >= 0 && request.Slot < player.Items.Length && player.Items[request.Slot] != null)
                        {
                            int firstEmptySlot = -1;
                            for (int i = 0; i < targetPlayer.Items.Length; i++)
                            {
                                if (targetPlayer.Items[i] == null)
                                {
                                    firstEmptySlot = i;
                                    break;
                                }
                            }

                            if (firstEmptySlot == -1)
                            {
                                return Results.BadRequest(new ErrorResponse("The recipient does not have space"));
                            }
                            else
                            {
                                if (player.Items[request.Slot] == null)
                                {
                                    return Results.BadRequest(new ErrorResponse("You can't give an item you don't have"));
                                }
                                targetPlayer.Items[firstEmptySlot] = player.Items[request.Slot];
                                player.Items[request.Slot] = null;
                                
                                player.Stats.UpdateFromItems(player.Items);
                                targetPlayer.Stats.UpdateFromItems(player.Items);
                                events.SendEvent(new PlayerInventoryUpdateEvent(player));
                                events.SendEvent(new PlayerInventoryUpdateEvent(targetPlayer));
                                events.SendEvent(new PlayerShareItemEvent(player, targetPlayer, targetPlayer.Items[firstEmptySlot]!));
                                return Results.Ok();
                            }
                        }
                        else
                        {
                            return Results.BadRequest(new ErrorResponse("Invalid inventory slot"));
                        }
                    }
                    else
                    {
                        return Results.BadRequest(new ErrorResponse("Target player doesn't exist"));
                    }
                    
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



    private IResult GetPlayerHandler(ISessionManager sessionManager, IGameManager gameManager, [FromBody] PlayerAuthRequest request)
    {
        if (sessionManager.IsSessionValid(request.SessionToken))
        {
            User user = sessionManager.GetAuthedUser(request.SessionToken);
            if (gameManager.GetGameState() == GameState.Active)
            {
                Player? player = gameManager.GetActiveGame().GetPlayer(user);
                if (player == null)
                {
                    return Results.NoContent();
                }
                else
                {
                    return Results.Ok(player);
                }
            }
            else
            {
                return Results.BadRequest("Game is not active");
            }
        }
        else
        {
            return Results.BadRequest("Invalid session token");
        }
    }

    private async Task<IResult> UseAbilityHandler(ISessionManager sessionManager,IUserRepository userRepository ,[FromServices] IGameManager gameManger, IEventManager events, [FromBody] AbilityRequest request)
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
                                events.SendEvent(new PlayerUseItemEvent(player, null, usableItem, Items.IsPositive(usableItem)));
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
                                    events.SendEvent(new PlayerUseItemEvent(player, targetPlayer, targetableItem, Items.IsPositive(targetableItem)));
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

    private async Task<IResult> PowerShareHandler (ISessionManager sessionManager, IUserRepository userRepository, [FromServices] IGameManager gameManger, IEventManager events, [FromBody] PowerShareRequest request)
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
                    User target;
                    try
                    {
                        target = await userRepository.GetUser(request.Username);
                    }
                    catch (UserNotFoundException e)
                    {
                        return Results.BadRequest("Invalid target");
                    }

                    if (target.Username == user.Username)
                    {
                        return Results.BadRequest("You cannot give power to yourself!");
                    }

                    Player? targetPlayer = currentGame.GetPlayer(target);
                    if (targetPlayer != null)
                    {
                        if (request.PowerAmount >= player.Stats.Power || request.PowerAmount <= 0)
                        {
                            return Results.BadRequest("Cannot give more power than you have!");
                        }
                        
                        int finalPowerAmount = (int)(Math.Min(request.PowerAmount + targetPlayer.Stats.Power, targetPlayer.Stats.CurrentStats[StatType.MaxPower]));

                        int actualExchange = finalPowerAmount - targetPlayer.Stats.Power;
                        player.Stats.Power -= actualExchange;
                        targetPlayer.Stats.Power += actualExchange;
                        events.SendEvent(new PlayerUpdatePowerEvent(player, player.Stats.Power));
                        events.SendEvent(new PlayerUpdatePowerEvent(targetPlayer, targetPlayer.Stats.Power));
                        events.SendEvent(new PlayerSharePowerEvent(player, targetPlayer, actualExchange));
                        
                        return Results.Ok();
                    }
                    else
                    {
                        return Results.BadRequest("Target player doesn't exist");
                    }
                    
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

    private IResult PlayerJoinHandler(ISessionManager sessionManager, [FromServices] IGameManager gameManager, IEventManager events, [FromBody] PlayerAuthRequest request)
    {
        if (sessionManager.IsSessionValid(request.SessionToken))
        {
            User user = sessionManager.GetAuthedUser(request.SessionToken);
            Player? player = gameManager.AddPlayer(user);
            if (player == null)
            {
                return Results.BadRequest(new ErrorResponse("Can't join player. Game is probably not initialized"));
            }
            return Results.Ok(new PlayerJoinResponse(player, user));
        }
        else
        {
            return Results.BadRequest(new ErrorResponse("Invalid Session"));
        }
        
    }
    
    private IResult CreateAllianceHandler(ISessionManager sessionManager, [FromServices] IGameManager gameManager, IEventManager events, [FromBody] AllianceRequest request)
    {
        if (sessionManager.IsSessionValid(request.SessionToken))
        {
            User user = sessionManager.GetAuthedUser(request.SessionToken);
            Game game = gameManager.GetCurrentGame();
            if (game?.State != GameState.Active)
            {
                return Results.BadRequest(new ErrorResponse("Game is not started"));
            }
            Player? player = game.GetPlayer(user);

            if (player == null)
            {
                return Results.BadRequest(new ErrorResponse("Player has not joined yet!"));
            }
                
            Alliance alliance = game.CreateAlliance(request.AllianceName, player)!;
            events.SendEvent(new AllianceCreatedEvent(alliance));
            return Results.Ok(new AllianceResponse(alliance));
        }
        else
        {
            return Results.BadRequest(new ErrorResponse("Invalid Session"));
        }
    }
    
    private IResult JoinAllianceHandler(ISessionManager sessionManager, [FromServices] IGameManager gameManager, IEventManager events, [FromBody] AllianceRequest request)
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
                Alliance alliance = gameManager.GetActiveGame().JoinAlliance(request.AllianceName, player)!;
                events.SendEvent(new PlayerJoinAllianceEvent(player,alliance));
                return Results.Ok(new AllianceResponse(alliance));
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
    
    private IResult LeaveAllianceHandler(ISessionManager sessionManager, [FromServices] IGameManager gameManager, IEventManager events, [FromBody] AllianceLeaveRequest request)
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
                
                Alliance? alliance = gameManager.GetActiveGame().LeaveAlliance(player);
                if (alliance == null)
                {
                    return Results.BadRequest(new ErrorResponse("Playe is not in an alliance"));
                }
                events.SendEvent(new PlayerLeaveAlliance(player, alliance));
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
    private IResult SellItemHandler(ISessionManager sessionManager, [FromServices] IGameManager gameManager, [FromServices] IItemRegistry itemRegistry, IEventManager events, [FromBody] ShopSellRequest request)
    {
        if (sessionManager.IsSessionValid(request.SessionToken))
        {
            User user = sessionManager.GetAuthedUser(request.SessionToken);
            Player? player = gameManager.GetActiveGame().GetPlayer(user);

            if (player == null)
            {
                Results.BadRequest("Player has not joined yet!");
            }
            else if (request.ItemSlot >= 0 && request.ItemSlot < player.Items.Length)
            {
                IItem? itemToSell = player.Items[request.ItemSlot];
                if (itemToSell == null)
                {
                    return Results.BadRequest(new ErrorResponse("Can't sell a nonexistant item!"));
                }
                else
                {
                    player.Stats.Power = int.Min((int)Math.Floor(itemToSell.Price * 0.7f)+player.Stats.Power, (int)player.Stats.CurrentStats[StatType.MaxPower]);
                    player.Items[request.ItemSlot] = null;
                    events.SendEvent(new PlayerInventoryUpdateEvent(player));
                    events.SendEvent(new PlayerUpdatePowerEvent(player, player.Stats.Power));
                    return Results.Ok();
                }
            }

            return Results.BadRequest("Unable to sell item!");
        }
        else
        {
            return Results.BadRequest(new ErrorResponse("Invalid Session"));
        }
    }
    private IResult ShopPurchaseHandler(ISessionManager sessionManager, [FromServices] IGameManager gameManager, [FromServices] IItemRegistry itemRegistry, IEventManager events, [FromBody] ShopPurchaseRequest request)
    {
        if (sessionManager.IsSessionValid(request.SessionToken))
        {
            User user = sessionManager.GetAuthedUser(request.SessionToken);
            Player? player = gameManager.GetActiveGame().GetPlayer(user);

            if (player == null)
            {
                Results.BadRequest("Player has not joined yet!");
            }

            IItem? preCopyItem = itemRegistry.Parse(request.ItemId);
            if (preCopyItem == null)
            {
                return Results.BadRequest(new ErrorResponse("Invalid ItemId"));
            }
            
            for (int i = 0; i < player!.Items.Length; i++)
            {
                if (player.Items[i] == null && player.Stats.Power > preCopyItem.Price + 1)
                {
                    player.Stats.Power -= preCopyItem.Price;
                    IItem copiedItem = DeepCopyUtils.DeepCopy(preCopyItem);
                    player.Stats.UpdateFromItems(player.Items!);

                    IInProgressItem inProgressItem = new InProgressItem(player, copiedItem);
                    player.Items[i] = inProgressItem;
                    
                    events.SendEvent(new PlayerUpdatePowerEvent(player, player.Stats.Power));
                    events.SendEvent(new PlayerInventoryUpdateEvent(player));
                    events.SendEvent(new PlayerStartMakingItemEvent(player, inProgressItem));
                    Utils.RunAfterDelay( ()=>
                    {
                        if (player.Items[i] != inProgressItem) return;
                        player.Items[i] = copiedItem;
                        events.SendEvent(new PlayerInventoryUpdateEvent(player));
                        events.SendEvent(new PlayerFinishMakingItemEvent(player, copiedItem));
                    }, TimeSpan.FromSeconds(copiedItem.BuildTime), app.Logger);
                    
                    return Results.Ok(new ShopPurchaseResponse(inProgressItem));
                }
            }
            return Results.BadRequest("Unable to purchase item!");
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
        return Results.Ok(itemRegistry.GetItems().Select(e => (object)e).ToList());
    }

    private IResult ValidateAuthHandler(ISessionManager sessionManager, [FromBody] ValidateAuthRequest request)
    {
        if (request.SessionToken == null)
        {
            return Results.BadRequest("Session token cannot be null");
        }

        try
        {
            return Results.Ok(sessionManager.IsSessionValid(request.SessionToken)
                ? new ValidateAuthResponse(true, sessionManager.GetAuthedUser(request.SessionToken))
                : new ValidateAuthResponse(false, null));
        }
        catch (SessionExpiredException e)
        {
            return Results.Ok(new ValidateAuthResponse(false, null));
        }
    }

    private async Task DropDatabase(INeo4jDataAccess neo4JDataAccess)
    {
        await neo4JDataAccess.ExecuteWriteTransactionAsync<bool>(@"MATCH (n) DETACH DELETE n RETURN true");
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