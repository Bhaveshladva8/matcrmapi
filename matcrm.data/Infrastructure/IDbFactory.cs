using System;
using matcrm.data.Context;

namespace matcrm.data
{
    public interface IDbFactory : IDisposable
    {
         OneClappContext Init();
    }
}