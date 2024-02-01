using AutoMapper;
using BackEnd.Repositories;

namespace BackEnd.Filters
{
    public class TestFilters : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var repositorioGeneros = context.Arguments.OfType<IRepositorioGeneros>().FirstOrDefault();
            var paramEntero = context.Arguments.OfType<int>().FirstOrDefault();
            var paramMapper = context.Arguments.OfType<IMapper>().FirstOrDefault();

            var result = await next(context);

            return result;
        }
    }
}
