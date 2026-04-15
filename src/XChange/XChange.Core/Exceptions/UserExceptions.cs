using System;
using System.Collections.Generic;
using System.Text;

namespace XChange.Core.Exceptions
{
    public class CreateUserException(string field) : CoreException($"El campo {field} es inválido.");
    public class PasswordAndGoogleIdException()
        : CoreException("No fue encontrada una forma de validación para que el usuario inicie sesión.");

}
