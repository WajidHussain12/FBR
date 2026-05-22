using System.Text.Json.Serialization;

namespace FBR_DI.Application.DTOs.FbrApi;

public class ProvinceDto
{
    [JsonPropertyName("stateProvinceCode")]
    public int StateProvinceCode { get; set; }

    [JsonPropertyName("stateProvinceDesc")]
    public string StateProvinceDesc { get; set; } = string.Empty;
}

public class DocTypeDto
{
    [JsonPropertyName("docTypeId")]
    public int DocTypeId { get; set; }

    [JsonPropertyName("docDescription")]
    public string DocDescription { get; set; } = string.Empty;
}

public class UomDto
{
    [JsonPropertyName("uoM_ID")]
    public int UoMId { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}

public class TransactionTypeDto
{
    [JsonPropertyName("transactioN_TYPE_ID")]
    public int TransactionTypeId { get; set; }

    [JsonPropertyName("transactioN_DESC")]
    public string TransactionDesc { get; set; } = string.Empty;
}

public class TaxRateDto
{
    [JsonPropertyName("ratE_ID")]
    public int RateId { get; set; }

    [JsonPropertyName("ratE_DESC")]
    public string RateDesc { get; set; } = string.Empty;

    [JsonPropertyName("ratE_VALUE")]
    public decimal RateValue { get; set; }
}

public class HsCodeDto
{
    [JsonPropertyName("hS_CODE")]
    public string HsCode { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}

public class StatlResponseDto
{
    [JsonPropertyName("status code")]
    public string StatusCode { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

public class RegistrationTypeResponseDto
{
    [JsonPropertyName("statuscode")]
    public string StatusCode { get; set; } = string.Empty;

    [JsonPropertyName("REGISTRATION_NO")]
    public string RegistrationNo { get; set; } = string.Empty;

    [JsonPropertyName("REGISTRATION_TYPE")]
    public string RegistrationType { get; set; } = string.Empty;
}
