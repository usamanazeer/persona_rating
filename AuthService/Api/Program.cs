using System.Reflection;
using AuthService.Application.Features.GoogleLogin;
using AuthService.Application.Features.RefreshToken;
using AuthService.Application.Features.GetCurrentUser;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Services;
using AuthService.Grpc;
using EasyNetQ;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Consul;
using Grpc.AspNetCore.Server;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));



// Entity Framework
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"), 
        b => b.MigrationsAssembly("AuthService.Api")));

// MediatR, FluentValidation, AutoMapper
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.RegisterServicesFromAssembly(typeof(AuthService.Application.Features.GoogleLogin.GoogleLoginCommand).Assembly);
});
builder.Services.AddValidatorsFromAssembly(typeof(AuthService.Application.Features.GoogleLogin.GoogleLoginValidator).Assembly);
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(AuthService.Application.Features.GoogleLogin.GoogleLoginCommand).Assembly);

// EasyNetQ (RabbitMQ)
builder.Services.AddSingleton(RabbitHutch.CreateBus("host=rabbitmq"));

// Consul.Net (placeholder for future service discovery)
builder.Services.AddSingleton<IConsulClient, ConsulClient>(_ => new ConsulClient(cfg =>
{
    cfg.Address = new Uri("http://consul:8500");
}));

// Services
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddHttpClient();

// JWT Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("super-secret-key"))
        };
    });

// Controllers
builder.Services.AddControllers();

// gRPC
builder.Services.AddGrpc();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Map gRPC service
app.MapGrpcService<UserGrpcService>();

app.Run();
