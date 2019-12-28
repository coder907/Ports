using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ports.Configuration
{
    public class ConfigException : Exception
    {
        public ConfigException(string message) : base(message)
        {
        }
    }
}