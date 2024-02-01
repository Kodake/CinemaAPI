using FluentValidation;

namespace BackEnd.Filters
{
    public class FiltroValidaciones<T> : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

            if (validator is null)
            {
                return await next(context);
            }

            var dto = context.Arguments.OfType<T>().FirstOrDefault();

            if (dto is null)
            {
                return TypedResults.Problem("No se pudo encontrar la entidad a validar");
            }

            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }

            return await next(context);
        }
    }
}
