using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.CommonUtils.Result
{
    public class NotFoundResultMessage : ResultMessage
    {
        public NotFoundResultMessage(string code) : base(code, ResultMessageLevels.Error)
        {
        }
    }
}
