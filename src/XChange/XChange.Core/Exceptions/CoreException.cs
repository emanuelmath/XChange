using System;
using System.Collections.Generic;
using System.Text;

namespace XChange.Core.Exceptions
{
    public class CoreException(string ex) : Exception(ex);
}
