using BackEnd.Entities;

namespace BackEnd.Repositories
{
    public interface IRepositorioGeneros
    {
        Task<List<Genero>> ObtenerTodos();
        Task<Genero?> ObtenerPorId(int id);
        Task<int> Crear(Genero genero);
        Task<bool> Existe(int id);
        Task Actualizar(Genero genero);
        Task Borrar(int id);
    }
}
