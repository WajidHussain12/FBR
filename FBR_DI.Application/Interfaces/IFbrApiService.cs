using FBR_DI.Application.DTOs.FbrApi;
using FBR_DI.Application.ResultPattern;

namespace FBR_DI.Application.Interfaces;

public interface IFbrApiService
{
    Task<Result<PostInvoiceResponseDto>> PostInvoiceAsync(
        PostInvoiceRequestDto request, string bearerToken,
        bool isSandbox, CancellationToken ct);

    Task<Result<PostInvoiceResponseDto>> ValidateInvoiceAsync(
        PostInvoiceRequestDto request, string bearerToken,
        bool isSandbox, CancellationToken ct);

    Task<Result<List<ProvinceDto>>> GetProvincesAsync(
        string bearerToken, CancellationToken ct);

    Task<Result<List<DocTypeDto>>> GetDocTypesAsync(
        string bearerToken, CancellationToken ct);

    Task<Result<List<UomDto>>> GetUomListAsync(
        string bearerToken, CancellationToken ct);

    Task<Result<List<TransactionTypeDto>>> GetTransactionTypesAsync(
        string bearerToken, CancellationToken ct);

    Task<Result<List<TaxRateDto>>> GetTaxRatesAsync(
        string bearerToken, string date, int transTypeId,
        int originationSupplier, CancellationToken ct);

    Task<Result<List<HsCodeDto>>> GetHsCodesAsync(
        string bearerToken, CancellationToken ct);

    Task<Result<StatlResponseDto>> CheckStatlAsync(
        string bearerToken, string regno, string date, CancellationToken ct);

    Task<Result<RegistrationTypeResponseDto>> GetRegistrationTypeAsync(
        string bearerToken, string registrationNo, CancellationToken ct);
}
