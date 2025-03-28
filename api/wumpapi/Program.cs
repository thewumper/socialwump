var builder = WebApplication.CreateBuilder(args);

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



app.Run();
