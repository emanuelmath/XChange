using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace XChange.Core.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
