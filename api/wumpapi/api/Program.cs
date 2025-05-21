using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using wumpapi.configuration;
using wumpapi.neo4j;
using wumpapi.Services;
using wumpapi.structures;
using SessionExpiredException = wumpapi.Services.SessionExpiredException;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
var settings = new ApplicationSettings();
builder.Configuration.GetSection("ApplicationSettings").Bind(settings);




// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton(GraphDatabase.Driver(settings.Neo4jConnection, AuthTokens.Basic(settings.Neo4jUser, settings.Neo4jPassword)));
builder.Services.AddScoped<INeo4jDataAccess, Neo4jDataAccess>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddSingleton<ISessionManager, SessionManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/login", async (ISessionManager sessionManager, IUserRepository userRepository, IPasswordHasher<User> passwordHasher,[FromBody] LoginUserRequest loginInfo) => {
  try
  {
    Tuple<string, User> sessiondata =
      await sessionManager.AuthUser(loginInfo.Username, loginInfo.Password, userRepository, passwordHasher);
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
}).WithName("login").Produces<LoginResponse>().Produces<ErrorResponse>(StatusCodes.Status401Unauthorized);

app.MapPost("/logout", (ISessionManager sessionManager, [FromBody] LogoutRequest logoutRequest) =>
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
}).WithName("logout").Produces<Ok>().Produces<ErrorResponse>(StatusCodes.Status401Unauthorized);


app.MapGet("/graph", (IUserRepository userRepository) =>
{
  return userRepository.GetGraph();
}).WithName("graph");
app.MapGet("/users", async (IUserRepository userRepository) => await userRepository.GetUsers()).WithName("users");



app.MapPost("/createaccount", async (IUserRepository userRepo, IPasswordHasher<User> passwordHasher, [FromBody] CreateUserRequest request) =>
{
  // Check for existing user
  if (await userRepo.UserExists(request.Username, request.Email))
  {
    return Results.Conflict(new ErrorResponse("A user with the given username already exists"));
  }

  User user = new User(request.Username, request.Email, request.FirstName, request.LastName);
  user.Password = passwordHasher.HashPassword(user,request.Password);
  
  bool successful = await userRepo.AddUser(user);
  if (!successful)
  {
    throw new Exception("Failed to create user");
  }
  return Results.Created();
}).WithName("CreateAccount")
.Produces(StatusCodes.Status201Created)
.Produces<ErrorResponse>(StatusCodes.Status409Conflict);

app.MapPost("/createRelationship", async (IUserRepository userRepository, ISessionManager sessionManager, [FromBody] CreateRelationshipRequest request) =>
{
  try
  {
    User initiator = sessionManager.GetAuthedUser(request.SessionToken);
    User target = await userRepository.GetUser(username: request.TargetUser);
    bool succesful = await userRepository.CreateRelationship(initiator, target,request.RelationshipName,request.Data);
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
}).WithName("CreateRelationship");

app.MapGet("/maxWantsADummyBecauseHeIsADummy", () => """
                                                     {
                                                       "nodes": [
                                                         {
                                                           "id": 1,
                                                           "name": "A"
                                                         },
                                                         {
                                                           "id": 2,
                                                           "name": "B"
                                                         },
                                                         {
                                                           "id": 3,
                                                           "name": "C"
                                                         },
                                                         {
                                                           "id": 4,
                                                           "name": "D"
                                                         },
                                                         {
                                                           "id": 5,
                                                           "name": "E"
                                                         },
                                                         {
                                                           "id": 6,
                                                           "name": "F"
                                                         },
                                                         {
                                                           "id": 7,
                                                           "name": "G"
                                                         },
                                                         {
                                                           "id": 8,
                                                           "name": "H"
                                                         },
                                                         {
                                                           "id": 9,
                                                           "name": "I"
                                                         },
                                                         {
                                                           "id": 10,
                                                           "name": "J"
                                                         }
                                                       ],
                                                       "links": [
                                                     
                                                         {
                                                           "source": 1,
                                                           "target": 2
                                                         },
                                                         {
                                                           "source": 1,
                                                           "target": 5
                                                         },
                                                         {
                                                           "source": 1,
                                                           "target": 6
                                                         },
                                                     
                                                         {
                                                           "source": 2,
                                                           "target": 3
                                                         },
                                                                 {
                                                           "source": 2,
                                                           "target": 7
                                                         }
                                                         ,
                                                     
                                                         {
                                                           "source": 3,
                                                           "target": 4
                                                         },
                                                          {
                                                           "source": 8,
                                                           "target": 3
                                                         }
                                                         ,
                                                         {
                                                           "source": 4,
                                                           "target": 5
                                                         }
                                                         ,
                                                     
                                                         {
                                                           "source": 4,
                                                           "target": 9
                                                         },
                                                         {
                                                           "source": 5,
                                                           "target": 10
                                                         }
                                                       ]

                                                     }
                                                     """).WithName("/maxWantsADummyBecauseHeIsADummy");

app.Run();


public record CreateUserRequest([Required] string Username, [Required] string Password, [Required] string FirstName, [Required] string LastName, [Required][EmailAddress] string Email);
public record LoginUserRequest([Optional] string Username, [Optional][EmailAddress] string Email, [Required] string Password);

public record LoginResponse(string SessionToken, User User);
public record ErrorResponse(string Message);
public record RelationshipCreatedResponse(Connection Connection);

public record LogoutRequest(string SessionToken);

public record CreateRelationshipRequest(string SessionToken, string TargetUser, string RelationshipName, string Data);