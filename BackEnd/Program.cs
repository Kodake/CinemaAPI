using BackEnd.Entities;
using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);
var allowOrigins = builder.Configuration.GetValue<string>("Origins")!;

// Inicio servicios

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(configuration =>
    {
        configuration.WithOrigins(allowOrigins).AllowAnyHeader().AllowAnyMethod();
    });

    opt.AddPolicy("Free", configuration =>
    {
        configuration.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddOutputCache();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// Fin servicios

var app = builder.Build();

// Inicio middlewares

app.UseSwagger();

app.UseSwaggerUI();

app.UseCors();

app.UseOutputCache();

app.MapGet("/", [EnableCors(policyName: "Free")] () => "¡Hola Mundo!");

app.MapGet("/generos", () =>
{
    var generos = new List<Genero>
    {
        new Genero
        {
            Id = 1,
            Nombre = "Drama"
        },
        new Genero
        {
            Id = 2,
            Nombre = "Acción"
        },
        new Genero
        {
            Id = 3,
            Nombre = "Comedia"
        }
    };

    return generos;
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)));

app.Run();