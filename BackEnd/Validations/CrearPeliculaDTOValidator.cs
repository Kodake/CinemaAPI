using BackEnd.DTOs;
using BackEnd.Helpers;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CrearPeliculaDTOValidator:AbstractValidator<CrearPeliculaDTO>
    {
        public CrearPeliculaDTOValidator()
        {
            RuleFor(x => x.Titulo).NotEmpty().WithMessage(Utilities.CampoRequeridoMensaje)
                .MaximumLength(100).WithMessage(Utilities.MaximunLengthMensaje);
        }
    }
}
