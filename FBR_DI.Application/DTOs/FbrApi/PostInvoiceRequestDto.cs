using System.Text.Json.Serialization;

namespace FBR_DI.Application.DTOs.FbrApi;

public class PostInvoiceRequestDto
{
    [JsonPropertyName("invoiceType")]
    public string InvoiceType { get; set; } = string.Empty;

    [JsonPropertyName("invoiceDate")]
    public string InvoiceDate { get; set; } = string.Empty;

    [JsonPropertyName("sellerNTNCNIC")]
    public string SellerNTNCNIC { get; set; } = string.Empty;

    [JsonPropertyName("sellerBusinessName")]
    public string SellerBusinessName { get; set; } = string.Empty;

    [JsonPropertyName("sellerProvince")]
    public string SellerProvince { get; set; } = string.Empty;

    [JsonPropertyName("sellerAddress")]
    public string SellerAddress { get; set; } = string.Empty;

    [JsonPropertyName("buyerNTNCNIC")]
    public string? BuyerNTNCNIC { get; set; }

    [JsonPropertyName("buyerBusinessName")]
    public string BuyerBusinessName { get; set; } = string.Empty;

    [JsonPropertyName("buyerProvince")]
    public string BuyerProvince { get; set; } = string.Empty;

    [JsonPropertyName("buyerAddress")]
    public string BuyerAddress { get; set; } = string.Empty;

    [JsonPropertyName("buyerRegistrationType")]
    public string BuyerRegistrationType { get; set; } = string.Empty;

    [JsonPropertyName("invoiceRefNo")]
    public string? InvoiceRefNo { get; set; }

    [JsonPropertyName("scenarioId")]
    public string? ScenarioId { get; set; }

    [JsonPropertyName("items")]
    public List<InvoiceItemRequestDto> Items { get; set; } = new();
}
