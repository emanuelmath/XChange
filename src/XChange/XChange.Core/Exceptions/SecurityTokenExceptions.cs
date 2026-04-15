using System;
using System.Collections.Generic;
using System.Text;

namespace XChange.Core.Exceptions
{
    public class CreateSecurityTokenException(string field) : CoreException($"El campo {field} es inválido");
}
