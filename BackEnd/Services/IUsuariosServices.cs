using Microsoft.AspNetCore.Identity;

namespace BackEnd.Services
{
    public interface IUsuariosServices
    {
        Task<IdentityUser?> ObtenerUsuario();
    }
}