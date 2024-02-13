namespace BackEnd.Helpers
{
    public class Utilities
    {
        public static string CampoRequeridoMensaje = "El campo {PropertyName} es requerido";
        public static string MaximunLengthMensaje = "El campo {PropertyName} debe tener menos de {MaxLength} caracteres";
        public static string PrimeraLetraMayusculasMensaje = "El campo {PropertyName} debe comenzar con mayúsculas";
        public static string EmailMensaje = "El campo {PropertyName} debe ser un email válido";

        public static string GreaterThanOrEqualToMensaje(DateTime fechaMinima) 
        {
            return "El campo {PropertyName} debe ser posterior a " + fechaMinima.ToString("yyyy-MM-dd");
        }


        public static bool PrimerLetraEnMayusculas(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return true;
            }

            var primeraLetra = valor[0].ToString();

            return primeraLetra == primeraLetra.ToUpper();
        }
    }
}
