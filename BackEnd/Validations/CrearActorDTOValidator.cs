using BackEnd.DTOs;
using BackEnd.Helpers;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CrearActorDTOValidator: AbstractValidator<CrearActorDTO>
    {
        public CrearActorDTOValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().WithMessage(Utilities.CampoRequeridoMensaje)
                .MaximumLength(100).WithMessage(Utilities.MaximunLengthMensaje);

            var fechaMinima = new DateTime(1900, 1, 1);

            RuleFor(x => x.FechaNacimiento).GreaterThanOrEqualTo(fechaMinima)
                .WithMessage(Utilities.GreaterThanOrEqualToMensaje(fechaMinima));
        }
    }
}
