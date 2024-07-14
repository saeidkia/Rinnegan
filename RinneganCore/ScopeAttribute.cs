using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace RinneganCore
{
    public class ScopeAttribute:Attribute
    {
        private readonly ServiceLifetime _serviceLifetime;
        internal ServiceLifetime ServiceLifetime => _serviceLifetime;

        public ScopeAttribute(ServiceLifetime serviceLifetime)
        {
            _serviceLifetime = serviceLifetime;
        }
    }
}
