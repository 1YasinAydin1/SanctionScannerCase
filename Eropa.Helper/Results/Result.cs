using System;

namespace Eropa.Helper.Results
{
    public class Result : IResult
    {
        public Result(bool success, string message) : this(success)
        {
            Message = message;
        }
        public Result(bool success)
        {
            Success = success;
        }
        public Result(bool success, List<string> errors)
        {
            Success = success;
            Errors = errors;
        }
        public bool Success { get; }
        public string Message { get; }
        public List<string> Errors { get; }
    }
}
