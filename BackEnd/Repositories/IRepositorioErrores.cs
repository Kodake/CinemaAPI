using BackEnd.Entities;
using Error = BackEnd.Entities.Error;

namespace BackEnd.Repositories
{
    public interface IRepositorioErrores
    {
        Task Crear(Error error);
    }
}