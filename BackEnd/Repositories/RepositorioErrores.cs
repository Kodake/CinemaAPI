﻿using BackEnd.Context;
using BackEnd.Entities;

namespace BackEnd.Repositories
{
    public class RepositorioErrores : IRepositorioErrores
    {
        private readonly ApplicationDbContext context;
        public RepositorioErrores(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task Crear(Error error)
        {
            context.Add(error);
            await context.SaveChangesAsync();
        }
    }
}
