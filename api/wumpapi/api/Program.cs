using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using wumpapi.configuration;
using wumpapi.neo4j;
using wumpapi.Services;
using wumpapi.structures;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
var settings = new ApplicationSettings();
builder.Configuration.GetSection("ApplicationSettings").Bind(settings);




// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton(GraphDatabase.Driver(settings.Neo4jConnection, AuthTokens.Basic(settings.Neo4jUser, settings.Neo4jPassword)));
builder.Services.AddSingleton<ISessionManager, SessionManager>();
builder.Services.AddScoped<INeo4jDataAccess, Neo4jDataAccess>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/login", () => {
  return "tokenOrSomething"; 
}).WithName("login");
app.MapGet("/logout", () => {
  return true;
}).WithName("logout");
app.MapGet("/graph", () => {
  return "graph data or something";
}).WithName("/graph");




app.MapPost("/createaccount", async (IUserRepository userRepo, IPasswordHasher<User> passwordHasher, [FromBody] CreateUserRequest request) =>
{
  // Check for existing user
  if (await userRepo.UserExists(request.Username, request.Email))
  {
    return Results.Conflict("Username or email already exists");
  }

  User user = new User(request.Username, request.Email, request.FirstName, request.LastName);
  user.Password = passwordHasher.HashPassword(user,request.Password);
  
  bool successful = await userRepo.AddUser(user);
  return successful ? Results.Created() : Results.BadRequest();
}).WithName("CreateAccount")
.Produces<User>(StatusCodes.Status201Created)
.ProducesValidationProblem()
.Produces(StatusCodes.Status409Conflict);

app.MapGet("/add", () =>
{ 
  return "success or fail";
}).WithName("/add");

app.MapGet("/maxWantsADummyBecauseHeIsADummy", () =>
{
  return """
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
         """;
}).WithName("/maxWantsADummyBecauseHeIsADummy");

app.Run();


public record CreateUserRequest([Required] string Username, [Required] string Password, [Required] string FirstName, [Required] string LastName, [Required][EmailAddress] string Email);

