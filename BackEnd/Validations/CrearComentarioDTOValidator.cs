using BackEnd.DTOs;
using BackEnd.Helpers;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CrearComentarioDTOValidator: AbstractValidator<CrearComentarioDTO>
    {
        public CrearComentarioDTOValidator()
        {
            RuleFor(x => x.Cuerpo).NotEmpty().WithMessage(Utilities.CampoRequeridoMensaje);
        }
    }
}
