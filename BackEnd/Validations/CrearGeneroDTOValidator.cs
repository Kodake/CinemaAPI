using BackEnd.DTOs;
using BackEnd.Repositories;
using BackEnd.Helpers;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CrearGeneroDTOValidator : AbstractValidator<CrearGeneroDTO>
    {
        public CrearGeneroDTOValidator(IRepositorioGeneros repositorio,
            IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = 0;

            if (routeId is string valorString)
            {
                int.TryParse(valorString, out id);
            }

            RuleFor(x => x.Nombre).NotEmpty().WithMessage(Utilities.CampoRequeridoMensaje)
            .MaximumLength(50).WithMessage(Utilities.MaximunLengthMensaje)
            .Must(Utilities.PrimerLetraEnMayusculas).WithMessage(Utilities.PrimeraLetraMayusculasMensaje)
            .MustAsync(async (nombre, _) =>
            {
                var existe = await repositorio.Existe(id, nombre);
                return !existe;
            }).WithMessage(x => $"Ya existe un género con el nombre {x.Nombre}");
        }
    }
}
