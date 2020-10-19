using System.Collections.Generic;

namespace SSRD.CommonUtils.Result
{
    public class ResultMessage
    {
        public string Code { get; set; }
        public ResultMessageLevels Level { get; set; }

        public ResultMessage(string code, ResultMessageLevels level)
        {
            Code = code;
            Level = level;
        }

        public virtual string ToMessage()
        {
            return Code;
        }

        public virtual string ToMessage(IDictionary<string, string> messages)
        {
            bool exists = messages.TryGetValue(Code, out string message);
            if(!exists)
            {
                return Code;
            }

            return message;
        }
    }
}
