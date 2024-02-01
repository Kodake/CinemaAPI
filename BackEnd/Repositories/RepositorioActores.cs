﻿using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    public class RepositorioActores : IRepositorioActores
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public RepositorioActores(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Actor>> ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = context.Actores.AsQueryable();
            await httpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            return await queryable
                .OrderBy(x => x.Nombre)
                .Paginar(paginacionDTO)
                .ToListAsync();
        }

        public async Task<Actor?> ObtenerPorId(int id)
        {
            return await context.Actores
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Actor>> ObtenerPorNombre(string nombre)
        {
            return await context.Actores
                .Where(a => a.Nombre.Contains(nombre))
                .OrderBy(x => x.Nombre)
                .ToListAsync();
        }

        public async Task<int> Crear(Actor actor)
        {
            context.Add(actor);
            await context.SaveChangesAsync();
            return actor.Id;
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Actores.AnyAsync(x => x.Id == id);
        }

        public async Task<List<int>> Existen(List<int> ids)
        {
            return await context.Actores.Where(x => ids.Contains(x.Id)).Select(a => a.Id).ToListAsync();
        }

        public async Task Actualizar(Actor actor)
        {
            context.Update(actor);
            await context.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await context.Actores.Where(x => x.Id == id).ExecuteDeleteAsync();
        }
    }
}
