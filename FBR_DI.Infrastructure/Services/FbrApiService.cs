using System.Net;
using System.Text;
using System.Text.Json;
using FBR_DI.Application.DTOs.FbrApi;
using FBR_DI.Application.Interfaces;
using FBR_DI.Application.ResultPattern;
using Microsoft.Extensions.Logging;

namespace FBR_DI.Infrastructure.Services;

public class FbrApiService : IFbrApiService
{
    private const string SandboxBaseUrl = "https://gw.fbr.gov.pk/di_data/v1/di/";
    private const string ProductionBaseUrl = "https://gw.fbr.gov.pk/di_data/v1/di/";
    private const string ReferenceBaseUrl = "https://gw.fbr.gov.pk/pdi/v1/";
    private const string ReferenceV2BaseUrl = "https://gw.fbr.gov.pk/pdi/v2/";
    private const string StatlBaseUrl = "https://gw.fbr.gov.pk/dist/v1/";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FbrApiService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public FbrApiService(IHttpClientFactory httpClientFactory, ILogger<FbrApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<Result<PostInvoiceResponseDto>> PostInvoiceAsync(
        PostInvoiceRequestDto request, string bearerToken, bool isSandbox, CancellationToken ct)
    {
        var suffix = isSandbox ? "_sb" : "";
        var url = $"{(isSandbox ? SandboxBaseUrl : ProductionBaseUrl)}postinvoicedata{suffix}";
        return await SendInvoiceRequestAsync(request, bearerToken, url, ct);
    }

    public async Task<Result<PostInvoiceResponseDto>> ValidateInvoiceAsync(
        PostInvoiceRequestDto request, string bearerToken, bool isSandbox, CancellationToken ct)
    {
        var suffix = isSandbox ? "_sb" : "";
        var url = $"{(isSandbox ? SandboxBaseUrl : ProductionBaseUrl)}validateinvoicedata{suffix}";
        return await SendInvoiceRequestAsync(request, bearerToken, url, ct);
    }

    private async Task<Result<PostInvoiceResponseDto>> SendInvoiceRequestAsync(
        PostInvoiceRequestDto request, string bearerToken, string url, CancellationToken ct)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("FbrApiClient");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");

            var json = JsonSerializer.Serialize(request, JsonOptions);
            _logger.LogInformation("Sending invoice request to {Url}", url);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content, ct);

            var responseBody = await response.Content.ReadAsStringAsync(ct);
            _logger.LogInformation("FBR response status: {StatusCode}", response.StatusCode);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result<PostInvoiceResponseDto>.Failure("FBR API: Unauthorized. Check your Bearer Token.", "UNAUTHORIZED");

            if (response.StatusCode == HttpStatusCode.InternalServerError)
                return Result<PostInvoiceResponseDto>.Failure("FBR API: Internal Server Error.", "SERVER_ERROR");

            if (!response.IsSuccessStatusCode)
                return Result<PostInvoiceResponseDto>.Failure($"FBR API error: {response.StatusCode}", "API_ERROR");

            var result = JsonSerializer.Deserialize<PostInvoiceResponseDto>(responseBody, JsonOptions);
            if (result is null)
                return Result<PostInvoiceResponseDto>.Failure("Failed to deserialize FBR response.");

            return Result<PostInvoiceResponseDto>.Success(result);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request error calling FBR API at {Url}", url);
            return Result<PostInvoiceResponseDto>.Failure($"Network error: {ex.Message}", "NETWORK_ERROR");
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "FBR API request timed out at {Url}", url);
            return Result<PostInvoiceResponseDto>.Failure("FBR API request timed out.", "TIMEOUT");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error calling FBR API at {Url}", url);
            return Result<PostInvoiceResponseDto>.Failure($"Unexpected error: {ex.Message}", "UNKNOWN_ERROR");
        }
    }

    public async Task<Result<List<ProvinceDto>>> GetProvincesAsync(string bearerToken, CancellationToken ct)
        => await GetReferenceDataAsync<List<ProvinceDto>>(bearerToken, $"{ReferenceBaseUrl}provinces", ct);

    public async Task<Result<List<DocTypeDto>>> GetDocTypesAsync(string bearerToken, CancellationToken ct)
        => await GetReferenceDataAsync<List<DocTypeDto>>(bearerToken, $"{ReferenceBaseUrl}doctypecode", ct);

    public async Task<Result<List<UomDto>>> GetUomListAsync(string bearerToken, CancellationToken ct)
        => await GetReferenceDataAsync<List<UomDto>>(bearerToken, $"{ReferenceBaseUrl}uom", ct);

    public async Task<Result<List<TransactionTypeDto>>> GetTransactionTypesAsync(string bearerToken, CancellationToken ct)
        => await GetReferenceDataAsync<List<TransactionTypeDto>>(bearerToken, $"{ReferenceBaseUrl}transtypecode", ct);

    public async Task<Result<List<TaxRateDto>>> GetTaxRatesAsync(
        string bearerToken, string date, int transTypeId, int originationSupplier, CancellationToken ct)
    {
        var url = $"{ReferenceV2BaseUrl}SaleTypeToRate?date={date}&transTypeId={transTypeId}&originationSupplier={originationSupplier}";
        return await GetReferenceDataAsync<List<TaxRateDto>>(bearerToken, url, ct);
    }

    public async Task<Result<List<HsCodeDto>>> GetHsCodesAsync(string bearerToken, CancellationToken ct)
        => await GetReferenceDataAsync<List<HsCodeDto>>(bearerToken, $"{ReferenceBaseUrl}itemdesccode", ct);

    public async Task<Result<StatlResponseDto>> CheckStatlAsync(
        string bearerToken, string regno, string date, CancellationToken ct)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("FbrApiClient");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");

            var payload = JsonSerializer.Serialize(new { regno, date });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{StatlBaseUrl}statl", content, ct);

            if (!response.IsSuccessStatusCode)
                return Result<StatlResponseDto>.Failure($"STATL API error: {response.StatusCode}");

            var body = await response.Content.ReadAsStringAsync(ct);
            var result = JsonSerializer.Deserialize<StatlResponseDto>(body, JsonOptions);
            return result is null
                ? Result<StatlResponseDto>.Failure("Failed to deserialize STATL response.")
                : Result<StatlResponseDto>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling STATL API");
            return Result<StatlResponseDto>.Failure($"Error: {ex.Message}");
        }
    }

    public async Task<Result<RegistrationTypeResponseDto>> GetRegistrationTypeAsync(
        string bearerToken, string registrationNo, CancellationToken ct)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("FbrApiClient");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");

            var payload = JsonSerializer.Serialize(new { Registration_No = registrationNo });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{StatlBaseUrl}Get_Reg_Type", content, ct);

            if (!response.IsSuccessStatusCode)
                return Result<RegistrationTypeResponseDto>.Failure($"Registration Type API error: {response.StatusCode}");

            var body = await response.Content.ReadAsStringAsync(ct);
            var result = JsonSerializer.Deserialize<RegistrationTypeResponseDto>(body, JsonOptions);
            return result is null
                ? Result<RegistrationTypeResponseDto>.Failure("Failed to deserialize Registration Type response.")
                : Result<RegistrationTypeResponseDto>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Get_Reg_Type API");
            return Result<RegistrationTypeResponseDto>.Failure($"Error: {ex.Message}");
        }
    }

    private async Task<Result<T>> GetReferenceDataAsync<T>(string bearerToken, string url, CancellationToken ct)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("FbrApiClient");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");

            _logger.LogInformation("Fetching reference data from {Url}", url);
            var response = await client.GetAsync(url, ct);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result<T>.Failure("FBR API: Unauthorized.", "UNAUTHORIZED");

            if (!response.IsSuccessStatusCode)
                return Result<T>.Failure($"FBR API error: {response.StatusCode}");

            var body = await response.Content.ReadAsStringAsync(ct);
            var result = JsonSerializer.Deserialize<T>(body, JsonOptions);
            return result is null
                ? Result<T>.Failure("Failed to deserialize response.")
                : Result<T>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching reference data from {Url}", url);
            return Result<T>.Failure($"Error: {ex.Message}");
        }
    }
}
