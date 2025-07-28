using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add configuration sources
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add Ocelot
builder.Services.AddOcelot(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure CORS
app.UseCors("AllowAll");

// ? Map health BEFORE Ocelot
app.MapGet("/health", () => new
{
    status = "healthy",
    service = "Gateway",
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
});

// ? Now apply Ocelot middleware
app.MapWhen(
    ctx => !ctx.Request.Path.StartsWithSegments("/health"),
    branchApp => branchApp.UseOcelot().Wait()
);

app.Run();
