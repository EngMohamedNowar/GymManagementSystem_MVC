using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Common
{
    public record Result(bool success,string?error=null,ResultKind Kind = ResultKind.ok) 
    {
        public static Result Ok() => new(true);
        public static Result Fail(string message, ResultKind kind = ResultKind.Conflict) => new(false,message,kind);
        public static Result NotFound(string message="Not Found",ResultKind kind = ResultKind.NotFound) => new(false, message, kind);
        public static Result Validation(string message, ResultKind kind = ResultKind.ValidationFailed) => new(false, message, kind);
    }
    public record Result<T>(bool success,T? value, string? error = null, ResultKind Kind = ResultKind.ok)
    {
        public static Result<T> Ok(T value) => new(true, value);
        public static Result<T> Fail(string message, ResultKind kind = ResultKind.Conflict) => new(false,default, message, kind);
        public static Result<T> NotFound(string message = "Not Found", ResultKind kind = ResultKind.NotFound) => new(false, default, message, kind);
        public static Result<T> Validation(string message, ResultKind kind = ResultKind.ValidationFailed) => new(false, default, message, kind);
    }
}
