using BackEnd.Context;
using BackEnd.Endpoints;
using BackEnd.Repositories;
using BackEnd.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var allowOrigins = builder.Configuration.GetValue<string>("Origins")!;

// Inicio servicios

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer("name=DefaultConnection"));

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
builder.Services.AddScoped<IRepositorioGeneros, RepositorioGeneros>();
builder.Services.AddScoped<IRepositorioActores, RepositorioActores>();
builder.Services.AddScoped<IRepositorioPeliculas, RepositorioPeliculas>();
builder.Services.AddScoped<IRepositorioComentarios, RepositorioComentarios>();
builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosAzure>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program));

// Fin servicios

var app = builder.Build();

// Inicio middlewares

app.UseSwagger();

app.UseSwaggerUI();

app.UseStaticFiles();

app.UseCors();

app.UseOutputCache();

app.MapGet("/", [EnableCors(policyName: "Free")] () => "¡Hola Mundo!");

app.MapGroup("/generos").MapGeneros();
app.MapGroup("/actores").MapActores();
app.MapGroup("/peliculas").MapPeliculas();
app.MapGroup("/pelicula/{peliculaId:int}/comentarios").MapComentarios();

app.Run();