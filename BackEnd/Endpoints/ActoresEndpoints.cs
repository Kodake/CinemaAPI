using AutoMapper;
using BackEnd.DTOs;
using BackEnd.Entities;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories;
using BackEnd.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.OpenApi.Models;

namespace BackEnd.Endpoints
{
    public static class ActoresEndpoints
    {
        private static readonly string contenedor = "actores";
        public static RouteGroupBuilder MapActores(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("actores-get"))
                .AgregarParametrosPaginacionOpenAPI();
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapGet("obtenerPorNombre/{nombre}", ObtenerPorNombre);
            group.MapPost("/", Crear).DisableAntiforgery()
                .AddEndpointFilter<FiltroValidaciones<CrearActorDTO>>()
                .RequireAuthorization("isAdmin")
                .WithOpenApi();
            group.MapPut("/{id:int}", Actualizar)
                .DisableAntiforgery()
                .AddEndpointFilter<FiltroValidaciones<CrearActorDTO>>()
                .RequireAuthorization("isAdmin")
                .WithOpenApi();
            group.MapDelete("/{id:int}", Borrar).RequireAuthorization("isAdmin");
            return group;
        }

        static async Task<Ok<List<ActorDTO>>> ObtenerTodos(IRepositorioActores repositorio,
            IMapper mapper, PaginacionDTO paginacion)
        {
            //var paginacion = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPorPagina };
            var actores = await repositorio.ObtenerTodos(paginacion);
            var actoresDTO = mapper.Map<List<ActorDTO>>(actores);
            return TypedResults.Ok(actoresDTO);
        }

        static async Task<Results<Ok<ActorDTO>, NotFound>> ObtenerPorId(IRepositorioActores repositorio,
            IMapper mapper, int id)
        {
            var actor = await repositorio.ObtenerPorId(id);

            if (actor is null)
            {
                return TypedResults.NotFound();
            }

            var actorDTO = mapper.Map<ActorDTO>(actor);
            return TypedResults.Ok(actorDTO);
        }

        static async Task<Ok<List<ActorDTO>>> ObtenerPorNombre(string nombre,
            IRepositorioActores repositorio,
            IMapper mapper)
        {
            var actores = await repositorio.ObtenerPorNombre(nombre);
            var actoresDTO = mapper.Map<List<ActorDTO>>(actores);
            return TypedResults.Ok(actoresDTO);
        }

        static async Task<Results<Created<ActorDTO>, ValidationProblem>> Crear([FromForm] CrearActorDTO crearActorDTO,
           IRepositorioActores repositorio,
           IOutputCacheStore outputCacheStore,
           IAlmacenadorArchivos almacenadorArchivos,
           IMapper mapper)
        {
            var actor = mapper.Map<Actor>(crearActorDTO);

            if (crearActorDTO.Foto is not null)
            {
                var url = await almacenadorArchivos.Almacenar(contenedor, crearActorDTO.Foto);
                actor.Foto = url;
            }

            var id = await repositorio.Crear(actor);
            await outputCacheStore.EvictByTagAsync("actores-get", default);
            var actorDTO = mapper.Map<ActorDTO>(actor);
            return TypedResults.Created($"/actores/{id}", actorDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id, [FromForm] CrearActorDTO crearActorDTO,
           IRepositorioActores repositorio,
           IOutputCacheStore outputCacheStore,
           IAlmacenadorArchivos almacenadorArchivos,
           IMapper mapper)
        {
            var actorDB = await repositorio.ObtenerPorId(id);

            if (actorDB is null)
            {
                return TypedResults.NotFound();
            }

            var actorParaActualizar = mapper.Map<Actor>(crearActorDTO);
            actorParaActualizar.Id = id;
            actorParaActualizar.Foto = actorDB.Foto;

            if (crearActorDTO.Foto is not null)
            {
                var url = await almacenadorArchivos.Editar(actorParaActualizar.Foto,
                    contenedor, crearActorDTO.Foto);

                actorParaActualizar.Foto = url;
            }

            await repositorio.Actualizar(actorParaActualizar);
            await outputCacheStore.EvictByTagAsync("actores-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent>> Borrar(int id,
             IRepositorioActores repositorio,
             IOutputCacheStore outputCacheStore,
             IAlmacenadorArchivos almacenadorArchivos)
        {
            var actorDB = await repositorio.ObtenerPorId(id);

            if (actorDB is null)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await almacenadorArchivos.Borrar(actorDB.Foto, contenedor);
            await outputCacheStore.EvictByTagAsync("actores-get", default);
            return TypedResults.NoContent();
        }
    }
}
