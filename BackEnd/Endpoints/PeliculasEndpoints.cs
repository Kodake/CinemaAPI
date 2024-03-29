﻿using AutoMapper;
using BackEnd.DTOs;
using BackEnd.Entities;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories;
using BackEnd.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class PeliculasEndpoints
    {
        private static readonly string contenedor = "peliculas";
        public static RouteGroupBuilder MapPeliculas(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("peliculas-get"))
                .AgregarParametrosPaginacionOpenAPI();
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPost("/", Crear).DisableAntiforgery()
                .AddEndpointFilter<FiltroValidaciones<CrearPeliculaDTO>>()
                .RequireAuthorization("isAdmin")
                .WithOpenApi();
            group.MapPut("/{id:int}", Actualizar).DisableAntiforgery()
                .AddEndpointFilter<FiltroValidaciones<CrearPeliculaDTO>>()
                .RequireAuthorization("isAdmin")
                .WithOpenApi();
            group.MapDelete("/{id:int}", Borrar).RequireAuthorization("isAdmin");
            group.MapPost("/{id:int}/asignar-generos", AsignarGeneros).RequireAuthorization("isAdmin");
            group.MapPost("/{id:int}/asignar-actores", AsignarActores).RequireAuthorization("isAdmin");
            group.MapGet("/filtrar", FiltrarPeliculas)
                .AgregarParametrosPeliculasFiltroOpenAPI();
            return group;
        }

        static async Task<Ok<List<PeliculaDTO>>> ObtenerTodos(IRepositorioPeliculas repositorio,
            IMapper mapper, PaginacionDTO paginacion)
        {
            //var paginacion = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPorPagina };
            var peliculas = await repositorio.ObtenerTodos(paginacion);
            var peliculasDTO = mapper.Map<List<PeliculaDTO>>(peliculas);
            return TypedResults.Ok(peliculasDTO);
        }

        static async Task<Results<Ok<PeliculaDTO>, NotFound>> ObtenerPorId(IRepositorioPeliculas repositorio,
            IMapper mapper, int id)
        {
            var pelicula = await repositorio.ObtenerPorId(id);

            if (pelicula is null)
            {
                return TypedResults.NotFound();
            }

            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
            return TypedResults.Ok(peliculaDTO);
        }

        static async Task<Ok<List<PeliculaDTO>>> ObtenerPorTitulo(string titulo,
            IRepositorioPeliculas repositorio,
            IMapper mapper)
        {
            var peliculas = await repositorio.ObtenerPorTitulo(titulo);
            var peliculasDTO = mapper.Map<List<PeliculaDTO>>(peliculas);
            return TypedResults.Ok(peliculasDTO);
        }

        static async Task<Created<PeliculaDTO>> Crear([FromForm] CrearPeliculaDTO crearPeliculaDTO,
           IRepositorioPeliculas repositorio,
           IOutputCacheStore outputCacheStore,
           IAlmacenadorArchivos almacenadorArchivos,
           IMapper mapper)
        {
            var pelicula = mapper.Map<Pelicula>(crearPeliculaDTO);

            if (crearPeliculaDTO.Poster is not null)
            {
                var url = await almacenadorArchivos.Almacenar(contenedor, crearPeliculaDTO.Poster);
                pelicula.Poster = url;
            }

            var id = await repositorio.Crear(pelicula);
            await outputCacheStore.EvictByTagAsync("peliculas-get", default);
            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
            return TypedResults.Created($"/peliculas/{id}", peliculaDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id, [FromForm] CrearPeliculaDTO crearPeliculaDTO,
           IRepositorioPeliculas repositorio,
           IOutputCacheStore outputCacheStore,
           IAlmacenadorArchivos almacenadorArchivos,
           IMapper mapper)
        {
            var peliculaDB = await repositorio.ObtenerPorId(id);

            if (peliculaDB is null)
            {
                return TypedResults.NotFound();
            }

            var peliculaParaActualizar = mapper.Map<Pelicula>(crearPeliculaDTO);
            peliculaParaActualizar.Id = id;
            peliculaParaActualizar.Poster = peliculaDB.Poster;

            if (crearPeliculaDTO.Poster is not null)
            {
                var url = await almacenadorArchivos.Editar(peliculaParaActualizar.Poster,
                    contenedor, crearPeliculaDTO.Poster);

                peliculaParaActualizar.Poster = url;
            }

            await repositorio.Actualizar(peliculaParaActualizar);
            await outputCacheStore.EvictByTagAsync("peliculas-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent>> Borrar(int id,
             IRepositorioPeliculas repositorio,
             IOutputCacheStore outputCacheStore,
             IAlmacenadorArchivos almacenadorArchivos)
        {
            var peliculaDB = await repositorio.ObtenerPorId(id);

            if (peliculaDB is null)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await almacenadorArchivos.Borrar(peliculaDB.Poster, contenedor);
            await outputCacheStore.EvictByTagAsync("peliculas-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound, BadRequest<string>>> AsignarGeneros(int id,
            List<int> generosIds,
            IRepositorioPeliculas repositorioPeliculas, IRepositorioGeneros repositorioGeneros)
        {
            if (!await repositorioPeliculas.Existe(id))
            {
                return TypedResults.NotFound();
            }

            var generosExistentes = new List<int>();

            if (generosIds.Count != 0)
            {
                generosExistentes = await repositorioGeneros.Existen(generosIds);
            }

            if (generosExistentes.Count != generosIds.Count)
            {
                var generosNoExistentes = generosIds.Except(generosExistentes);

                return TypedResults.BadRequest($"Los géneros de id {string.Join(",", generosNoExistentes)} no existen.");
            }

            await repositorioPeliculas.AsignarGeneros(id, generosIds);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent, BadRequest<string>>> AsignarActores(int id,
            List<AsignarActorPeliculaDTO> actoresDTO, IRepositorioPeliculas repositorioPeliculas,
            IRepositorioActores repositorioActores, IMapper mapper)
        {
            if (!await repositorioPeliculas.Existe(id))
            {
                return TypedResults.NotFound();
            }

            var actoresExistentes = new List<int>();
            var actoresIds = actoresDTO.Select(a => a.ActorId).ToList();

            if (actoresDTO.Count != 0)
            {
                actoresExistentes = await repositorioActores.Existen(actoresIds);
            }

            if (actoresExistentes.Count != actoresDTO.Count)
            {
                var actoresNoExistentes = actoresIds.Except(actoresExistentes);

                return TypedResults.BadRequest($"Los actores de id {string.Join(",", actoresNoExistentes)} no existen");
            }

            var actores = mapper.Map<List<ActorPelicula>>(actoresDTO);

            await repositorioPeliculas.AsignarActores(id, actores);
            return TypedResults.NoContent();
        }

        static async Task<Ok<List<PeliculaDTO>>> FiltrarPeliculas(PeliculasFiltrarDTO peliculasFiltrarDTO,
            IRepositorioPeliculas repositorio, IMapper mapper)
        {
            var peliculas = await repositorio.Filtrar(peliculasFiltrarDTO);
            var peliculasDTO = mapper.Map<List<PeliculaDTO>>(peliculas);
            return TypedResults.Ok(peliculasDTO);
        }
    }
}
