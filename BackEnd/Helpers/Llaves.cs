using Microsoft.IdentityModel.Tokens;

namespace BackEnd.Helpers
{
    public static class Llaves
    {
        public const string IssuerPropio = "cinema-API";
        public const string SeccionLlaves = "Authentication:Schemes:Bearer:SigninKeys";
        public const string SeccionLlaves_Emisor = "Issuer";
        public const string SeccionLlaves_Valor = "Value";

        public static IEnumerable<SecurityKey> ObtenerLlave(IConfiguration configuration)
            => ObtenerLlave(configuration, IssuerPropio);

        public static IEnumerable<SecurityKey> ObtenerLlave(IConfiguration configuration, string issuer)
        {
            var signingKey = configuration.GetSection(SeccionLlaves)
                .GetChildren()
                .SingleOrDefault(llave => llave[SeccionLlaves_Emisor] == issuer);

            if (signingKey is not null && signingKey[SeccionLlaves_Valor] is string valorLlave)
            {
                yield return new SymmetricSecurityKey(Convert.FromBase64String(valorLlave));
            }
        }

        public static IEnumerable<SecurityKey> ObtenerTodasLasLlaves(IConfiguration configuration)
        {
            var signingKeys = configuration.GetSection(SeccionLlaves)
                .GetChildren();

            foreach (var signingKey in signingKeys)
            {
                if (signingKey[SeccionLlaves_Valor] is string valorLlave)
                {
                    yield return new SymmetricSecurityKey(Convert.FromBase64String(valorLlave));
                }
            }
        }
    }
}
