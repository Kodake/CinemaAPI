using AutoMapper;
using BackEnd.DTOs;
using BackEnd.Entities;
using BackEnd.Filters;
using BackEnd.Repositories;
using BackEnd.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class ComentariosEndpoints
    {
        public static RouteGroupBuilder MapComentarios(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos)
                .CacheOutput(c =>
                c.Expire(TimeSpan.FromSeconds(60))
                .Tag("comentarios-get")
                .SetVaryByRouteValue(new string[] { "peliculaId" }));
            group.MapGet("/{id:int}", ObtenerPorId);

            group.MapPost("/", Crear).AddEndpointFilter<FiltroValidaciones<CrearComentarioDTO>>()
                .RequireAuthorization();

            group.MapPut("/{id:int}", Actualizar)
                .AddEndpointFilter<FiltroValidaciones<CrearComentarioDTO>>()
                .RequireAuthorization();

            group.MapDelete("/{id:int}", Borrar).RequireAuthorization();

            return group;
        }

        static async Task<Results<Ok<List<ComentarioDTO>>, NotFound>> ObtenerTodos(int peliculaId,
            IRepositorioComentarios repositorioComentarios, IRepositorioPeliculas repositorioPeliculas,
            IMapper mapper)
        {
            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            var comentarios = await repositorioComentarios.ObtenerTodos(peliculaId);
            var comentariosDTO = mapper.Map<List<ComentarioDTO>>(comentarios);
            return TypedResults.Ok(comentariosDTO);
        }

        static async Task<Results<Ok<ComentarioDTO>, NotFound>> ObtenerPorId(int peliculaId, int id,
            IRepositorioComentarios repositorio, IMapper mapper)
        {
            var comentario = await repositorio.ObtenerPorId(id);

            if (comentario is null)
            {
                return TypedResults.NotFound();
            }

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return TypedResults.Ok(comentarioDTO);
        }

        static async Task<Results<Created<ComentarioDTO>, NotFound, BadRequest<string>>> Crear(int peliculaId,
            CrearComentarioDTO crearComentarioDTO, IRepositorioComentarios repositorioComentarios,
            IRepositorioPeliculas repositorioPeliculas, IMapper mapper, IOutputCacheStore outputCacheStore,
            IUsuariosServices usuariosServices)
        {
            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            var comentario = mapper.Map<Comentario>(crearComentarioDTO);
            comentario.PeliculaId = peliculaId;

            var usuario = await usuariosServices.ObtenerUsuario();

            if (usuario is null)
            {
                return TypedResults.BadRequest("Usuario no encontrado");
            }

            comentario.UsuarioId = usuario.Id;

            var id = await repositorioComentarios.Crear(comentario);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return TypedResults.Created($"/comentario/{id}", comentarioDTO);
        }

        static async Task<Results<NoContent, NotFound, ForbidHttpResult>> Actualizar(int peliculaId, int id,
            CrearComentarioDTO crearComentarioDTO, IOutputCacheStore outputCacheStore,
            IRepositorioComentarios repositorioComentarios, IRepositorioPeliculas repositorioPeliculas,
            UsuariosServices usuariosServices)
        {
            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            var comentarioDB = await repositorioComentarios.ObtenerPorId(id);

            if (comentarioDB is null)
            {
                return TypedResults.NotFound();       
            }

            var usuario = await usuariosServices.ObtenerUsuario();

            if (usuario is null)
            {
                return TypedResults.NotFound();
            }

            if (comentarioDB.UsuarioId != usuario.Id)
            {
                return TypedResults.Forbid();
            }

            comentarioDB.Cuerpo = crearComentarioDTO.Cuerpo;

            await repositorioComentarios.Actualizar(comentarioDB);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound, ForbidHttpResult>> Borrar(int peliculaId, int id,
            IRepositorioComentarios repositorio, IOutputCacheStore outputCacheStore, IUsuariosServices usuariosServices)
        {
            var comentarioDB = await repositorio.ObtenerPorId(id);

            if (comentarioDB is null)
            {
                return TypedResults.NotFound();
            }

            var usuario = await usuariosServices.ObtenerUsuario();

            if (usuario is null)
            {
                return TypedResults.NotFound();
            }

            if (comentarioDB.UsuarioId != usuario.Id)
            {
                return TypedResults.Forbid();
            }

            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }
    }
}
