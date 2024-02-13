using BackEnd.DTOs;
using BackEnd.Helpers;
using FluentValidation;

namespace BackEnd.Validations
{
    public class EditarClaimDTOValidator: AbstractValidator<EditarClaimDTO>
    {
        public EditarClaimDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(Utilities.CampoRequeridoMensaje)
                .MaximumLength(256).WithMessage(Utilities.MaximunLengthMensaje)
                .EmailAddress().WithMessage(Utilities.EmailMensaje);
        }
    }
}
