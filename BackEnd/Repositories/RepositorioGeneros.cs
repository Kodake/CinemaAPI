﻿using BackEnd.Context;
using BackEnd.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    public class RepositorioGeneros : IRepositorioGeneros
    {
        private readonly ApplicationDbContext context;

        public RepositorioGeneros(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Genero>> ObtenerTodos()
        {
            return await context.Generos
                .OrderBy(x => x.Nombre)
                .ToListAsync();
        }

        public async Task<Genero?> ObtenerPorId(int id)
        {
            return await context.Generos.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> Crear(Genero genero)
        {
            context.Add(genero);
            await context.SaveChangesAsync();
            return genero.Id;
        }

        public async Task Actualizar(Genero genero)
        {
            context.Update(genero);
            await context.SaveChangesAsync();
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Generos.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> Existe(int id, string nombre)
        {
            return await context.Generos.AnyAsync(x => x.Id != id && x.Nombre == nombre);
        }

        public async Task<List<int>> Existen(List<int> ids)
        {
            return await context.Generos.Where(x => ids.Contains(x.Id)).Select(x => x.Id).ToListAsync();
        }

        public async Task Borrar(int id)
        {
            await context.Generos.Where(x => x.Id == id).ExecuteDeleteAsync();
        }
    }
}
