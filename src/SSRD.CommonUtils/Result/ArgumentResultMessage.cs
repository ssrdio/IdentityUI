using System.Collections.Generic;

namespace SSRD.CommonUtils.Result
{
    public class ArgumentResultMessage : ResultMessage
    {
        public object[] Arguments { get; set; }

        public ArgumentResultMessage(string code, ResultMessageLevels level, object[] arguments) : base(code, level)
        {
            Arguments = arguments;
        }

        public override string ToMessage()
        {
            return string.Format(Code, Arguments);
        }

        public override string ToMessage(IDictionary<string, string> messages)
        {
            bool exists = messages.TryGetValue(Code, out string message);
            if (!exists)
            {
                return string.Format(Code, Arguments);
            }

            return string.Format(message, Arguments);
        }
    }
}
