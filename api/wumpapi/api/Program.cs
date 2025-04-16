using Neo4j.Driver;
using wumpapi.configuration;
using wumpapi.neo4j;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
var settings = new ApplicationSettings();
builder.Configuration.GetSection("ApplicationSettings").Bind(settings);
builder.Services.AddSingleton(GraphDatabase.Driver(settings.Neo4jConnection, AuthTokens.Basic(settings.Neo4jUser, settings.Neo4jPassword)));
builder.Services.AddScoped<INeo4jDataAccess, Neo4jDataAccess>();
builder.Services.AddTransient<IPersonRepository, PersonRepository>();







// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/login", () =>
    {
        return "tokenOrSomething";
    })
    .WithName("login");
app.MapGet("/logout", () =>
    {
        return true;
    })
    .WithName("logout");
app.MapGet("/graph", () =>
    {
        return "graph data or something";
    }).WithName("/graph");
app.MapGet("/createaccount", () =>
    {
        return "success or fail";
    }).WithName("/createaccount");
app.MapGet("/add", () =>
    {
        return "success or fail";
    }).WithName("/add");

app.Run();


