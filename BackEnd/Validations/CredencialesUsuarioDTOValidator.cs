using BackEnd.DTOs;
using BackEnd.Helpers;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CredencialesUsuarioDTOValidator: AbstractValidator<CredencialesUsuarioDTO>
    {
        public CredencialesUsuarioDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(Utilities.CampoRequeridoMensaje)
                .MaximumLength(256).WithMessage(Utilities.MaximunLengthMensaje)
                .EmailAddress().WithMessage(Utilities.EmailMensaje);

            RuleFor(x => x.Password).NotEmpty().WithMessage(Utilities.CampoRequeridoMensaje);
        }
    }
}
