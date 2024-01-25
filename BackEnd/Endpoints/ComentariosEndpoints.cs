using AutoMapper;
using BackEnd.DTOs;
using BackEnd.Entities;
using BackEnd.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class ComentariosEndpoints
    {
        public static RouteGroupBuilder MapComentarios(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos)
                .CacheOutput(c => c.Expire(TimeSpan.FromMinutes(100))
                .Tag("comentarios-get")
                .SetVaryByRouteValue(new string[] { "PeliculaId" }));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPost("/", Crear);
            group.MapPut("/{id:int}", Actualizar);
            group.MapDelete("/{id:int}", Borrar);
            return group;
        }

        static async Task<Results<Ok<List<ComentarioDTO>>, NotFound>> ObtenerTodos(int peliculaId,
            CrearComentarioDTO crearComentarioDTO, IRepositorioComentarios repositorioComentarios,
            IRepositorioPeliculas repositorioPeliculas, IMapper mapper)
        {
            var existe = await repositorioPeliculas.Existe(peliculaId);

            if (!existe)
            {
                return TypedResults.NotFound();
            }

            var comentarios = await repositorioComentarios.ObtenerTodos(peliculaId);
            var comentariosDTO = mapper.Map<List<ComentarioDTO>>(comentarios);
            return TypedResults.Ok(comentariosDTO);
        }

        static async Task<Results<Ok<ComentarioDTO>, NotFound>> ObtenerPorId(int peliculaId, int id,
            IRepositorioComentarios repositorio,
            IMapper mapper)
        {
            var comentario = await repositorio.ObtenerPorId(id);

            if (comentario is null)
            {
                return TypedResults.NotFound();
            }

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return TypedResults.Ok(comentarioDTO);
        }

        static async Task<Results<Created<ComentarioDTO>, NotFound>> Crear(int peliculaId,
            CrearComentarioDTO crearComentarioDTO, IRepositorioComentarios repositorioComentarios,
            IRepositorioPeliculas repositorioPeliculas, IMapper mapper, IOutputCacheStore outputCacheStore)
        {
            var existe = await repositorioPeliculas.Existe(peliculaId);

            if (!existe)
            {
                return TypedResults.NotFound();
            }

            var comentario = mapper.Map<Comentario>(crearComentarioDTO);
            comentario.PeliculaId = peliculaId;
            var id = await repositorioComentarios.Crear(comentario);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return TypedResults.Created($"/comentario/{id}", comentarioDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int peliculaId, int id,
            CrearComentarioDTO crearComentarioDTO, IOutputCacheStore outputCacheStore,
            IRepositorioComentarios repositorioComentarios, IRepositorioPeliculas repositorioPeliculas,
            IMapper mapper)
        {
            var existePelicula = await repositorioPeliculas.Existe(peliculaId);

            if (!existePelicula)
            {
                return TypedResults.NotFound();
            }

            var existeComentario = await repositorioComentarios.Existe(id);

            if (!existeComentario)
            {
                return TypedResults.NotFound();
            }

            var comentario = mapper.Map<Comentario>(crearComentarioDTO);
            comentario.Id = id;
            comentario.PeliculaId = peliculaId;

            await repositorioComentarios.Actualizar(comentario);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Borrar(int id,
            IRepositorioComentarios repositorio,
            IOutputCacheStore outputCacheStore)
        {
            var existeComentario = await repositorio.Existe(id);

            if (!existeComentario)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }
    }
}
