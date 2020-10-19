using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.CommonUtils.Result
{
    public class PropertyResultMessage : ResultMessage
    {
        public string PropertyName { get; set; }

        public PropertyResultMessage(string code, ResultMessageLevels level, string propertyName) : base(code, level)
        {
            PropertyName = propertyName;
        }
    }
}