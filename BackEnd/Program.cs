using BackEnd.Context;
using BackEnd.Endpoints;
using BackEnd.Entities;
using BackEnd.Repositories;
using BackEnd.Services;
using FluentValidation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Diagnostics;
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
builder.Services.AddScoped<IRepositorioErrores, RepositorioErrores>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddProblemDetails();

// Fin servicios

var app = builder.Build();

// Inicio middlewares

app.UseSwagger();

app.UseSwaggerUI();

app.UseExceptionHandler(exApp => exApp.Run(async context =>
{
    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
    var exception = exceptionHandlerFeature?.Error!;

    var error = new Error
    {
        Fecha = DateTime.UtcNow,
        MensajeDeError = exception.Message,
        StackTrace = exception.StackTrace
    };

    var repositorio = context.RequestServices.GetRequiredService<IRepositorioErrores>();
    await repositorio.Crear(error);

    await TypedResults.BadRequest(
        new { tipo = "error", mensaje = "Ha ocurrido un mensaje de error inespoerado", estatus = 500 })
    .ExecuteAsync(context);
}));
app.UseStatusCodePages();

app.UseStaticFiles();

app.UseCors();

app.UseOutputCache();

app.MapGet("/", [EnableCors(policyName: "Free")] () => "¡Hola Mundo!");

app.MapGroup("/generos").MapGeneros();
app.MapGroup("/actores").MapActores();
app.MapGroup("/peliculas").MapPeliculas();
app.MapGroup("/pelicula/{peliculaId:int}/comentarios").MapComentarios();

app.Run();