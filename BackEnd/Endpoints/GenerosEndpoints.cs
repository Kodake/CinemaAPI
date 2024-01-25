using AutoMapper;
using BackEnd.DTOs;
using BackEnd.Entities;
using BackEnd.Migrations;
using BackEnd.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class GenerosEndpoints
    {
        public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos)
                .CacheOutput(c => c.Expire(TimeSpan.FromMinutes(100)).Tag("generos-get"));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPost("/", Crear);
            group.MapPut("/{id:int}", Actualizar);
            group.MapDelete("/{id:int}", Borrar);
            return group;
        }
        static async Task<Ok<List<GeneroDTO>>> ObtenerTodos(IRepositorioGeneros repositorio, 
            IMapper mapper)
        {
            var generos = await repositorio.ObtenerTodos();
            var generosDTO = mapper.Map<List<GeneroDTO>>(generos);
            return TypedResults.Ok(generosDTO);
        }

        static async Task<Results<Ok<GeneroDTO>, NotFound>> ObtenerPorId(IRepositorioGeneros repositorio, 
            IMapper mapper, int id)
        {
            var genero = await repositorio.ObtenerPorId(id);

            if (genero is null)
            {
                return TypedResults.NotFound();
            }

            var generoDTO = mapper.Map<GeneroDTO>(genero);
            return TypedResults.Ok(generoDTO);
        }

        static async Task<Created<GeneroDTO>> Crear(CrearGeneroDTO crearGeneroDTO, 
            IRepositorioGeneros repositorio,
            IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {
            var genero = mapper.Map<Genero>(crearGeneroDTO);
            var id = await repositorio.Crear(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            var generoDTO = mapper.Map<GeneroDTO>(genero); 
            return TypedResults.Created($"/generos/{id}", generoDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id, CrearGeneroDTO crearGeneroDTO,
            IRepositorioGeneros repositorio,
            IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {
            var existe = await repositorio.Existe(id);

            if (!existe)
            {
                return TypedResults.NotFound();
            }

            var genero = mapper.Map<Genero>(crearGeneroDTO);
            genero.Id = id;
            await repositorio.Actualizar(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent>> Borrar(int id, IRepositorioGeneros repositorio,
             IOutputCacheStore outputCacheStore)
        {
            var existe = await repositorio.Existe(id);

            if (!existe)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }

    }
}
