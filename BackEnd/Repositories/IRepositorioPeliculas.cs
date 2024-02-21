using BackEnd.DTOs;
using BackEnd.Entities;

namespace BackEnd.Repositories
{
    public interface IRepositorioPeliculas
    {
        Task Actualizar(Pelicula pelicula);
        Task AsignarActores(int id, List<ActorPelicula> actores);
        Task AsignarGeneros(int id, List<int> generosIds);
        Task Borrar(int id);
        Task<int> Crear(Pelicula pelicula);
        Task<bool> Existe(int id);
        Task<Pelicula?> ObtenerPorId(int id);
        Task<List<Pelicula>> ObtenerPorTitulo(string titulo);
        Task<List<Pelicula>> ObtenerTodos(PaginacionDTO paginacionDTO);
        Task<List<Pelicula>> Filtrar(PeliculasFiltrarDTO peliculasFiltrarDTO);
    }
}