using AutoMapper;
using BackEnd.Repositories;

namespace BackEnd.DTOs
{
    public class ObtenerGeneroPorIdPeticionDTO
    {
        public int Id {  get; set; }

        public required IRepositorioGeneros  Repositorio { get; set; }
        public required IMapper Mapper { get; set; }
    }
}
