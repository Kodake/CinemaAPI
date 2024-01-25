﻿using AutoMapper;
using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities;
using BackEnd.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    public class RepositorioPeliculas : IRepositorioPeliculas
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly HttpContext httpContext;

        public RepositorioPeliculas(ApplicationDbContext context, 
            IHttpContextAccessor httpContextAccessor, 
            IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Pelicula>> ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = context.Peliculas.AsQueryable();
            await httpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            return await queryable
                .OrderBy(x => x.Titulo)
                .Paginar(paginacionDTO)
                .ToListAsync();
        }

        public async Task<Pelicula?> ObtenerPorId(int id)
        {
            return await context.Peliculas
                .Include(p => p.Comentarios)
                .Include(p => p.GenerosPeliculas)
                    .ThenInclude(gp => gp.Genero)
                .Include(p => p.ActoresPeliculas.OrderBy(a => a.Orden))
                    .ThenInclude(ap => ap.Actor)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Pelicula>> ObtenerPorTitulo(string titulo)
        {
            return await context.Peliculas
                .Where(a => a.Titulo.Contains(titulo))
                .OrderBy(x => x.Titulo)
                .ToListAsync();
        }

        public async Task<int> Crear(Pelicula pelicula)
        {
            context.Add(pelicula);
            await context.SaveChangesAsync();
            return pelicula.Id;
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Peliculas.AnyAsync(x => x.Id == id);
        }

        public async Task Actualizar(Pelicula pelicula)
        {
            context.Update(pelicula);
            await context.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await context.Peliculas.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task AsignarGeneros(int id, List<int> generosIds)
        {
            var pelicula = await context.Peliculas
                .Include(p => p.GenerosPeliculas)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pelicula is null)
            {
                throw new ArgumentException($"No existe una pelìcula con el id {id}");
            }

            var generosPeliculas = generosIds.Select(generoId => new GeneroPelicula() { GeneroId = generoId });

            pelicula.GenerosPeliculas = mapper.Map(generosPeliculas, pelicula.GenerosPeliculas);

            await context.SaveChangesAsync();
        }

        public async Task AsignarActores(int id, List<ActorPelicula> actores)
        {
            for (int i = 1; i <= actores.Count; i++)
            {
                actores[i - 1].Orden = i;
            }

            var pelicula = await context.Peliculas
                .Include(p => p.ActoresPeliculas)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pelicula is null)
            {
                throw new ArgumentException($"No existe la película con id: {id}");
            }

            pelicula.ActoresPeliculas = mapper.Map(actores, pelicula.ActoresPeliculas);

            await context.SaveChangesAsync();
        }
    }
}