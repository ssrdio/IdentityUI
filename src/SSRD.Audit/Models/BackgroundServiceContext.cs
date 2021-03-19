using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.Audit.Models
{
    public class BackgroundServiceContext
    {
        public string Name { get; }
        public string Identifier { get; }
        public IDictionary<string, string> Claims { get; set; }

        public BackgroundServiceContext(string name)
        {
            Name = name;
            Identifier = new Guid().ToString();
        }
    }
}
