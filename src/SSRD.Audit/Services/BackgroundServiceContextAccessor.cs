using SSRD.Audit.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SSRD.Audit.Services
{
    public class BackgroundServiceContextAccessor : IBackgroundServiceContextAccessor
    {
        private static AsyncLocal<BackgroundServiceContext> _currentContext = new AsyncLocal<BackgroundServiceContext>();

        public BackgroundServiceContext BackgroundServiceContext
        {
            get
            {
                return _currentContext.Value;
            }
            set
            {
                if(_currentContext.Value != null)
                {
                    _currentContext.Value = null;
                }

                if(value != null)
                {
                    _currentContext.Value = value;
                }
            }
        }

        public static void Create(string name)
        {
            new BackgroundServiceContextAccessor
            {
                BackgroundServiceContext = new BackgroundServiceContext(name)
            };
        }

        public static void Create(BackgroundServiceContext backgroundServiceContext)
        {
            new BackgroundServiceContextAccessor
            {
                BackgroundServiceContext = backgroundServiceContext
            };
        }
    }
}
