using AutoMapper;
using Message_app_backend.Entities;
using Message_app_backend.Mapper;
using Message_app_backend.RealTime;
using Message_app_backend.Repository;
using Message_app_backend.Repository.Implement;
using Message_app_backend.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Reflection;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Mapper = Message_app_backend.Mapper.Mapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDb>(
        option =>
        {
            option.UseSqlServer(builder.Configuration["SqlServer:ConnectionString"]);
        });
//builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:AccessToken_key"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/messagehub")))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// Add scope
Assembly assembly = Assembly.GetExecutingAssembly();
foreach (var type in assembly.ExportedTypes)
{
    if (type.BaseType != null && type.BaseType.Name.StartsWith("BaseRepositoryImpl"))
    {
        builder.Services.AddScoped(type);
    }
    else if (type.BaseType != null && type.BaseType.Name.StartsWith("BaseService"))
    {
        builder.Services.AddScoped(type);
    }
}

// Add redis
var multiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
{
    EndPoints = { builder.Configuration["Redis:Endpoint"] },
    User =  builder.Configuration["Redis:User"] ,
    Password = builder.Configuration["Redis:Password"]

});

builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

//Add auto mapper
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new Mapper());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // For mobile apps, allow http traffic.
    app.UseHttpsRedirection();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<MessageHub>("/messagehub");
});

app.MapControllers();

app.Run();
