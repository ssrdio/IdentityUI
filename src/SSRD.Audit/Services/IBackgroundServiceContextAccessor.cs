using SSRD.Audit.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.Audit.Services
{
    public interface IBackgroundServiceContextAccessor
    {
        BackgroundServiceContext BackgroundServiceContext { get; set; }
    }
}
