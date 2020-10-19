using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRD.CommonUtils.Result
{
    public class Result
    {
        public IEnumerable<ResultMessage> ResultMessages { get; private set; }

        public bool Success { get { return ResultMessages.All(x => x.Level == ResultMessageLevels.Success); } }
        public bool Failure { get { return ResultMessages.Any(x => x.Level == ResultMessageLevels.Error); } }

        public bool HasWarnings { get { return ResultMessages.Any(x => x.Level == ResultMessageLevels.Warning); } }

        public Result()
        {
            ResultMessages = new List<ResultMessage>();
        }

        public Result(IEnumerable<ResultMessage> messages)
        {
            ResultMessages = messages;
        }

        public Result(ResultMessage resultMessage)
        {
            ResultMessages = new List<ResultMessage> { resultMessage };
        }

        public static Result Ok()
        {
            return new Result();
        }

        public static Result Ok(string code)
        {
            ResultMessage resultMessage = new ResultMessage(
                code: code,
                level: ResultMessageLevels.Success);

            return new Result(resultMessage);
        }

        public static Result Ok(string code, params object[] arguments)
        {
            ArgumentResultMessage argumentResultMessage = new ArgumentResultMessage(
                code: code,
                level: ResultMessageLevels.Success,
                arguments: arguments);

            return new Result(argumentResultMessage);
        }

        public static Result Fail(string code)
        {
            ResultMessage resultMessage = new ResultMessage(
                code: code,
                level: ResultMessageLevels.Error);

            return new Result(resultMessage);
        }

        public static Result Fail(string code, params object[] arguments)
        {
            ArgumentResultMessage argumentResultMessage = new ArgumentResultMessage(
                code: code,
                level: ResultMessageLevels.Error,
                arguments: arguments);

            return new Result(argumentResultMessage);
        }

        public static Result Fail(ResultMessage resultMessage)
        {
            return new Result(resultMessage);
        }

        public static Result Fail(IEnumerable<ResultMessage> resultMessages)
        {
            return new Result(resultMessages);
        }

        public static Result Fail(IEnumerable<ResultMessage> resultMessages, IEnumerable<ResultMessage> resultMessages1)
        {
            return new Result(resultMessages.Concat(resultMessages1));
        }

        public static Result Fail(Result result)
        {
            return new Result(result.ResultMessages);
        }

        public static Result Warning(string code)
        {
            ResultMessage resultMessage = new ResultMessage(
                code: code,
                level: ResultMessageLevels.Warning);

            return new Result(resultMessage);
        }

        public static Result Warning(string code, params object[] arguments)
        {
            ArgumentResultMessage resultMessage = new ArgumentResultMessage(
                code: code,
                level: ResultMessageLevels.Warning,
                arguments: arguments);

            return new Result(resultMessage);
        }

        public static Result Add(Result result, Result result1)
        {
            return new Result(result.ResultMessages.Concat(result1.ResultMessages));
        }

        public static Result<T> Ok<T>(T value)
        {
            ResultMessage resultMessage = new ResultMessage(
                code: null,
                level: ResultMessageLevels.Success);

            return new Result<T>(resultMessage, value);
        }

        public static Result<T> Ok<T>(T value, string code)
        {
            ResultMessage resultMessage = new ResultMessage(
                code: code,
                level: ResultMessageLevels.Success);

            return new Result<T>(resultMessage, value);
        }

        public static Result<T> Ok<T>(T value, string code, params object[] arguments)
        {
            ArgumentResultMessage resultMessage = new ArgumentResultMessage(
                code: code,
                level: ResultMessageLevels.Success,
                arguments: arguments);

            return new Result<T>(resultMessage, value);
        }

        public static Result<T> Fail<T>(string code)
        {
            ResultMessage resultMessage = new ResultMessage(
                code: code,
                level: ResultMessageLevels.Error);

            return new Result<T>(resultMessage, default);
        }

        public static Result<T> Fail<T>(string code, params object[] arguments)
        {
            ArgumentResultMessage resultMessage = new ArgumentResultMessage(
                code: code,
                level: ResultMessageLevels.Error,
                arguments: arguments);

            return new Result<T>(resultMessage, default);
        }

        public static Result<T> Fail<T>(ResultMessage resultMessage)
        {
            return new Result<T>(resultMessage, default);
        }

        public static Result<T> Fail<T>(IEnumerable<ResultMessage> resultMessages)
        {
            return new Result<T>(resultMessages, default);
        }

        public static Result<T> Fail<T>(Result result)
        {
            return new Result<T>(result.ResultMessages, default);
        }

        public static Result<T> NotFound<T>()
        {
            return new Result<T>(new NotFoundResultMessage(string.Empty), default);
        }

        public static Result<T> NotFound<T>(string code)
        {
            return new Result<T>(new NotFoundResultMessage(code), default);
        }

        public static Result<T> Warning<T>(string code)
        {
            ResultMessage resultMessage = new ResultMessage(
                code: code,
                level: ResultMessageLevels.Warning);

            return new Result<T>(resultMessage, default);
        }

        public static Result<T> Warning<T>(string code, params object[] arguments)
        {
            ArgumentResultMessage resultMessage = new ArgumentResultMessage(
                code: code,
                level: ResultMessageLevels.Warning,
                arguments: arguments);

            return new Result<T>(resultMessage, default);
        }

        public static Result<T> Add<T>(Result result, Result result1)
        {
            return new Result<T>(result.ResultMessages.Concat(result1.ResultMessages), default);
        }
    }

    public class Result<T> : Result, IDisposable
    {
        public T Value { get; set; }

        public Result(ResultMessage resultMessage, T value) : base(resultMessage)
        {
            Value = value;
        }

        public Result(IEnumerable<ResultMessage> resultMessages, T value) : base(resultMessages)
        {
            Value = value;
        }

        public void Dispose()
        {
            if (Value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
