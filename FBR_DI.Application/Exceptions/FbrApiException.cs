using System.Net;

namespace FBR_DI.Application.Exceptions;

public class FbrApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string? FbrErrorCode { get; }
    public string? FbrErrorMessage { get; }

    public FbrApiException(HttpStatusCode statusCode, string message,
        string? fbrErrorCode = null, string? fbrErrorMessage = null)
        : base(message)
    {
        StatusCode = statusCode;
        FbrErrorCode = fbrErrorCode;
        FbrErrorMessage = fbrErrorMessage;
    }
}
