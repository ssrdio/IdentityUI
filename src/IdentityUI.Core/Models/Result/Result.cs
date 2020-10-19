using FluentValidation.Results;
using SSRD.IdentityUI.Core.Helper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Models.Result
{
    [Obsolete("Use SSRD.CommonUtils.Result.Result")]
    public class Result
    {
        public bool Success { get; private set; }
        public List<ResultError> Errors { get; private set; }

        protected Result(bool success, List<ResultError> errors)
        {
            Success = success;
            Errors = errors;
        }

        public bool Failure
        {
            get { return !Success; }
        }

        public static Result Ok()
        {
            return new Result(true, null);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, true, null);
        }

        public static Result Fail(List<ResultError> errors)
        {
            return new Result(false, errors);
        }

        public static Result Fail(IList<ValidationFailure> errors)
        {
            return new Result(false, ResultUtils.ToResultError(errors));
        }

        public static Result Fail(IEnumerable<IdentityError> errors)
        {
            return new Result(false, ResultUtils.ToResultError(errors));
        }

        public static Result<T> Fail<T>(List<ResultError> errors)
        {
            return new Result<T>(default(T), false, errors);
        }

        public static Result<T> Fail<T>(IList<ValidationFailure> errors)
        {
            return new Result<T>(default(T), false, ResultUtils.ToResultError(errors));
        }

        public static Result<T> Fail<T>(IEnumerable<IdentityError> errors)
        {
            return new Result<T>(default(T), false, ResultUtils.ToResultError(errors));
        }

        public static Result Fail(string code, string message)
        {
            return new Result(false, new List<ResultError> { new ResultError(code, message) });
        }

        public static Result<T> Fail<T>(string code, string message)
        {
            return new Result<T>(default(T), false, new List<ResultError> { new ResultError(code, message) });
        }

        public static Result Fail(string code, string message, string propertyName)
        {
            return new Result(false, new List<ResultError> { new ResultError(code, message, propertyName) });
        }

        public static Result<T> Fail<T>(string code, string message, string propertyName)
        {
            return new Result<T>(default(T), false, new List<ResultError> { new ResultError(code, message, propertyName) });
        }

        public class ResultError
        {
            public  string Code { get; }
            public  string Message { get; }
            public string PropertyName { get; }

            public ResultError(string code, string message)
            {
                Code = code;
                Message = message;
                PropertyName = null;
            }

            public ResultError(string code, string message, string propertyName)
            {
                Code = code;
                Message = message;
                PropertyName = propertyName;
            }
        }
    }

    [Obsolete("Use SSRD.CommonUtils.Result.Result")]
    public class Result<T> : Result
    {
        public T Value { get; private set; }

        protected internal Result(T value, bool success, List<ResultError> errors) : base(success, errors)
        {
            Value = value;
        }
    }
}
