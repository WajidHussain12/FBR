using System.Text.Json.Serialization;

namespace FBR_DI.Application.DTOs.FbrApi;

public class InvoiceItemRequestDto
{
    [JsonPropertyName("hsCode")]
    public string HsCode { get; set; } = string.Empty;

    [JsonPropertyName("productDescription")]
    public string ProductDescription { get; set; } = string.Empty;

    [JsonPropertyName("rate")]
    public string Rate { get; set; } = string.Empty;

    [JsonPropertyName("uoM")]
    public string UoM { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("totalValues")]
    public decimal TotalValues { get; set; }

    [JsonPropertyName("valueSalesExcludingST")]
    public decimal ValueSalesExcludingST { get; set; }

    [JsonPropertyName("fixedNotifiedValueOrRetailPrice")]
    public decimal FixedNotifiedValueOrRetailPrice { get; set; }

    [JsonPropertyName("salesTaxApplicable")]
    public decimal SalesTaxApplicable { get; set; }

    [JsonPropertyName("salesTaxWithheldAtSource")]
    public decimal SalesTaxWithheldAtSource { get; set; }

    [JsonPropertyName("extraTax")]
    public decimal ExtraTax { get; set; }

    [JsonPropertyName("furtherTax")]
    public decimal FurtherTax { get; set; }

    [JsonPropertyName("sroScheduleNo")]
    public string? SroScheduleNo { get; set; }

    [JsonPropertyName("fedPayable")]
    public decimal FedPayable { get; set; }

    [JsonPropertyName("discount")]
    public decimal Discount { get; set; }

    [JsonPropertyName("saleType")]
    public string SaleType { get; set; } = string.Empty;

    [JsonPropertyName("sroItemSerialNo")]
    public string? SroItemSerialNo { get; set; }
}
