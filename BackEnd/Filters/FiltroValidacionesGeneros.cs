using BackEnd.DTOs;
using FluentValidation;

namespace BackEnd.Filters
{
    public class FiltroValidacionesGeneros : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var validator = context.HttpContext.RequestServices.GetService<IValidator<CrearGeneroDTO>>();

            if (validator is null)
            {
                return await next(context);
            }

            var generoDTO = context.Arguments.OfType<CrearGeneroDTO>().FirstOrDefault();

            if (generoDTO is null)
            {
                return TypedResults.Problem("No se pudo encontrar la entidad a validar");
            }

            var validationResult = await validator.ValidateAsync(generoDTO);

            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }

            return await next(context);
        }
    }
}
