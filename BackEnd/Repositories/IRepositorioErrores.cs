using BackEnd.Entities;

namespace BackEnd.Repositories
{
    public interface IRepositorioErrores
    {
        Task Crear(Error error);
    }
}