using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SSRD.Audit.Data
{
    public enum ActionTypes
    {
        Get = 10,
        [Description("Error")]
        BadRequest = 11,
        Add = 20,
        Update = 30,
        Delete = 40,
    }
}
