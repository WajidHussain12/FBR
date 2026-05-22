using System.Text.Json.Serialization;

namespace FBR_DI.Application.DTOs.FbrApi;

public class PostInvoiceResponseDto
{
    [JsonPropertyName("invoiceNumber")]
    public string? InvoiceNumber { get; set; }

    [JsonPropertyName("dated")]
    public string? Dated { get; set; }

    [JsonPropertyName("validationResponse")]
    public ValidationResponseDto ValidationResponse { get; set; } = new();
}

public class ValidationResponseDto
{
    [JsonPropertyName("statusCode")]
    public string StatusCode { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("invoiceStatuses")]
    public List<InvoiceStatusDto>? InvoiceStatuses { get; set; }
}

public class InvoiceStatusDto
{
    [JsonPropertyName("itemSNo")]
    public string ItemSNo { get; set; } = string.Empty;

    [JsonPropertyName("statusCode")]
    public string StatusCode { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("invoiceNo")]
    public string? InvoiceNo { get; set; }

    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }
}
