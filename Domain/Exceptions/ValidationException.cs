using System.Net;

namespace Domain.Exceptions;

public class ValidationException:AppException
{
    public ValidationException(string message, List<string> errors)
        : base(message, errors, HttpStatusCode.BadRequest)
    {
    }

    public ValidationException(List<string> errors)
        : base("Se encontraron errores de validación.", errors, HttpStatusCode.BadRequest)
    {
    }

}
