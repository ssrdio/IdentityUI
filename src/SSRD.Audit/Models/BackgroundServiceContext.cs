using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.Audit.Models
{
    public class BackgroundServiceContext
    {
        public string Name { get; }

        public BackgroundServiceContext(string name)
        {
            Name = name;
        }
    }
}
