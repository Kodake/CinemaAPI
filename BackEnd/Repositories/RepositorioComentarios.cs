using BackEnd.Context;
using BackEnd.Entities;
using BackEnd.Migrations;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    public class RepositorioComentarios : IRepositorioComentarios
    {
        private readonly ApplicationDbContext context;

        public RepositorioComentarios(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Comentario>> ObtenerTodos(int peliculaId)
        {
            return await context.Comentarios.Where(c => c.PeliculaId == peliculaId).ToListAsync();
        }

        public async Task<Comentario?> ObtenerPorId(int id)
        {
            return await context.Comentarios.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<int> Crear(Comentario comentario)
        {
            context.Add(comentario);
            await context.SaveChangesAsync();
            return comentario.Id;
        }

        public async Task Actualizar(Comentario comentario)
        {
            context.Update(comentario);
            await context.SaveChangesAsync();
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Peliculas.AnyAsync(x => x.Id == id);
        }

        public async Task Borrar(int id)
        {
            await context.Comentarios.Where(x => x.Id == id).ExecuteDeleteAsync();
        }
    }
}
