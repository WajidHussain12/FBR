namespace FBR_DI.Application.ResultPattern;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public T? Data { get; private set; }
    public string? Message { get; private set; }
    public string? ErrorCode { get; private set; }
    public List<string> Errors { get; private set; } = new();

    private Result() { }

    public static Result<T> Success(T data, string? message = null)
        => new() { IsSuccess = true, Data = data, Message = message };

    public static Result<T> Failure(string message, string? errorCode = null)
        => new() { IsSuccess = false, Message = message, ErrorCode = errorCode, Errors = new List<string> { message } };

    public static Result<T> Failure(List<string> errors)
        => new() { IsSuccess = false, Errors = errors, Message = errors.FirstOrDefault() };

    public static Result<T> NotFound(string message)
        => new() { IsSuccess = false, Message = message, ErrorCode = "NOT_FOUND", Errors = new List<string> { message } };
}
