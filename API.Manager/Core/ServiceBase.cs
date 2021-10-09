using API.Manager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Manager.Core
{
    public abstract class ServiceBase
    {
        protected ServiceBase()
        {

        }

        protected virtual async Task<T> FromResult<T>(T type)
        {
            return await Task.FromResult(type);
        }
    }
}
