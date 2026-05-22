using FBR_DI.Application.DTOs.FbrApi;
using FBR_DI.Application.Interfaces;
using FBR_DI.Application.ResultPattern;

namespace FBR_DI.Infrastructure.Services;

public class MockFbrApiService : IFbrApiService
{
    private static readonly Random _rng = new();

    // ──────────────────────────────────────────────────────────────────────────
    // POST INVOICE
    // ──────────────────────────────────────────────────────────────────────────
    public async Task<Result<PostInvoiceResponseDto>> PostInvoiceAsync(
        PostInvoiceRequestDto request, string bearerToken,
        bool isSandbox, CancellationToken ct)
    {
        await Task.Delay(500, ct);

        var unixTs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var fbrInvoiceNumber = $"{request.SellerNTNCNIC}DI{unixTs}";
        var dated = DateTime.UtcNow.ToString("yyyy-MM-dd");

        bool isValid = _rng.NextDouble() < 0.80;

        if (isValid)
        {
            var itemStatuses = request.Items
                .Select((_, i) => new InvoiceStatusDto
                {
                    ItemSNo    = (i + 1).ToString(),
                    StatusCode = "00",
                    Status     = "Valid",
                    InvoiceNo  = $"{fbrInvoiceNumber}-{(i + 1):D2}",
                    ErrorCode  = null,
                    Error      = null
                })
                .ToList();

            return Result<PostInvoiceResponseDto>.Success(new PostInvoiceResponseDto
            {
                InvoiceNumber = fbrInvoiceNumber,
                Dated         = dated,
                ValidationResponse = new ValidationResponseDto
                {
                    StatusCode      = "00",
                    Status          = "Valid",
                    ErrorCode       = null,
                    Error           = null,
                    InvoiceStatuses = itemStatuses
                }
            });
        }
        else
        {
            var itemStatuses = request.Items
                .Select((_, i) => new InvoiceStatusDto
                {
                    ItemSNo    = (i + 1).ToString(),
                    StatusCode = "01",
                    Status     = "In-Valid",
                    InvoiceNo  = null,
                    ErrorCode  = "0046",
                    Error      = "Provide rate."
                })
                .ToList();

            return Result<PostInvoiceResponseDto>.Success(new PostInvoiceResponseDto
            {
                InvoiceNumber = null,
                Dated         = dated,
                ValidationResponse = new ValidationResponseDto
                {
                    StatusCode      = "01",
                    Status          = "In-Valid",
                    ErrorCode       = "0046",
                    Error           = "Provide rate.",
                    InvoiceStatuses = itemStatuses
                }
            });
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // VALIDATE INVOICE (no invoiceNumber returned)
    // ──────────────────────────────────────────────────────────────────────────
    public async Task<Result<PostInvoiceResponseDto>> ValidateInvoiceAsync(
        PostInvoiceRequestDto request, string bearerToken,
        bool isSandbox, CancellationToken ct)
    {
        await Task.Delay(300, ct);

        bool isValid = _rng.NextDouble() < 0.80;

        if (isValid)
        {
            var itemStatuses = request.Items
                .Select((_, i) => new InvoiceStatusDto
                {
                    ItemSNo    = (i + 1).ToString(),
                    StatusCode = "00",
                    Status     = "Valid",
                    InvoiceNo  = null,
                    ErrorCode  = null,
                    Error      = null
                })
                .ToList();

            return Result<PostInvoiceResponseDto>.Success(new PostInvoiceResponseDto
            {
                InvoiceNumber = null,
                Dated         = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                ValidationResponse = new ValidationResponseDto
                {
                    StatusCode      = "00",
                    Status          = "Valid",
                    ErrorCode       = null,
                    Error           = null,
                    InvoiceStatuses = itemStatuses
                }
            });
        }
        else
        {
            var itemStatuses = request.Items
                .Select((_, i) => new InvoiceStatusDto
                {
                    ItemSNo    = (i + 1).ToString(),
                    StatusCode = "01",
                    Status     = "In-Valid",
                    InvoiceNo  = null,
                    ErrorCode  = "0046",
                    Error      = "Provide rate."
                })
                .ToList();

            return Result<PostInvoiceResponseDto>.Success(new PostInvoiceResponseDto
            {
                InvoiceNumber = null,
                Dated         = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                ValidationResponse = new ValidationResponseDto
                {
                    StatusCode      = "01",
                    Status          = "In-Valid",
                    ErrorCode       = "0046",
                    Error           = "Provide rate.",
                    InvoiceStatuses = itemStatuses
                }
            });
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // PROVINCES
    // ──────────────────────────────────────────────────────────────────────────
    public Task<Result<List<ProvinceDto>>> GetProvincesAsync(
        string bearerToken, CancellationToken ct)
    {
        var provinces = new List<ProvinceDto>
        {
            new() { StateProvinceCode =  7, StateProvinceDesc = "PUNJAB" },
            new() { StateProvinceCode =  8, StateProvinceDesc = "SINDH" },
            new() { StateProvinceCode =  9, StateProvinceDesc = "KHYBER PAKHTUNKHWA" },
            new() { StateProvinceCode = 10, StateProvinceDesc = "BALOCHISTAN" },
            new() { StateProvinceCode = 11, StateProvinceDesc = "AJK" },
            new() { StateProvinceCode = 12, StateProvinceDesc = "GILGIT-BALTISTAN" },
            new() { StateProvinceCode = 13, StateProvinceDesc = "ICT" }
        };
        return Task.FromResult(Result<List<ProvinceDto>>.Success(provinces));
    }

    // ──────────────────────────────────────────────────────────────────────────
    // DOC TYPES
    // ──────────────────────────────────────────────────────────────────────────
    public Task<Result<List<DocTypeDto>>> GetDocTypesAsync(
        string bearerToken, CancellationToken ct)
    {
        var types = new List<DocTypeDto>
        {
            new() { DocTypeId = 4, DocDescription = "Sale Invoice" },
            new() { DocTypeId = 9, DocDescription = "Debit Note" }
        };
        return Task.FromResult(Result<List<DocTypeDto>>.Success(types));
    }

    // ──────────────────────────────────────────────────────────────────────────
    // UOM LIST
    // ──────────────────────────────────────────────────────────────────────────
    public Task<Result<List<UomDto>>> GetUomListAsync(
        string bearerToken, CancellationToken ct)
    {
        var uoms = new List<UomDto>
        {
            new() { UoMId =  1, Description = "Numbers, pieces, units" },
            new() { UoMId =  2, Description = "KG" },
            new() { UoMId =  3, Description = "Gram" },
            new() { UoMId =  4, Description = "Litre" },
            new() { UoMId =  5, Description = "Metre" },
            new() { UoMId =  6, Description = "Square Metre" },
            new() { UoMId =  7, Description = "Cubic Metre" },
            new() { UoMId =  8, Description = "Ton" },
            new() { UoMId =  9, Description = "Dozen" },
            new() { UoMId = 10, Description = "Box" },
            new() { UoMId = 11, Description = "Pair" },
            new() { UoMId = 12, Description = "Set" },
            new() { UoMId = 13, Description = "KWH" },
            new() { UoMId = 14, Description = "MTR" },
            new() { UoMId = 77, Description = "Square Meter" }
        };
        return Task.FromResult(Result<List<UomDto>>.Success(uoms));
    }

    // ──────────────────────────────────────────────────────────────────────────
    // TRANSACTION TYPES
    // ──────────────────────────────────────────────────────────────────────────
    public Task<Result<List<TransactionTypeDto>>> GetTransactionTypesAsync(
        string bearerToken, CancellationToken ct)
    {
        var types = new List<TransactionTypeDto>
        {
            new() { TransactionTypeId =  18, TransactionDesc = "Local Supply of Goods" },
            new() { TransactionTypeId =  82, TransactionDesc = "Services Rendered Locally" },
            new() { TransactionTypeId =  87, TransactionDesc = "Export of Goods" },
            new() { TransactionTypeId = 111, TransactionDesc = "Import of Goods" },
            new() { TransactionTypeId = 150, TransactionDesc = "Supply of Electricity / Gas" }
        };
        return Task.FromResult(Result<List<TransactionTypeDto>>.Success(types));
    }

    // ──────────────────────────────────────────────────────────────────────────
    // TAX RATES
    // ──────────────────────────────────────────────────────────────────────────
    public Task<Result<List<TaxRateDto>>> GetTaxRatesAsync(
        string bearerToken, string date, int transTypeId,
        int originationSupplier, CancellationToken ct)
    {
        var rates = new List<TaxRateDto>
        {
            new() { RateId = 280, RateDesc = "0%",  RateValue = 0m },
            new() { RateId = 281, RateDesc = "5%",  RateValue = 5m },
            new() { RateId = 282, RateDesc = "18%", RateValue = 18m },
            new() { RateId = 734, RateDesc = "18% along with rupees 60 per kilogram", RateValue = 18m }
        };
        return Task.FromResult(Result<List<TaxRateDto>>.Success(rates));
    }

    // ──────────────────────────────────────────────────────────────────────────
    // HS CODES
    // ──────────────────────────────────────────────────────────────────────────
    public Task<Result<List<HsCodeDto>>> GetHsCodesAsync(
        string bearerToken, CancellationToken ct)
    {
        var codes = new List<HsCodeDto>
        {
            new() { HsCode = "0101.2100", Description = "HORSES - PURE-BRED BREEDING ANIMALS" },
            new() { HsCode = "0302.1100", Description = "TROUT FRESH OR CHILLED" },
            new() { HsCode = "1001.1000", Description = "DURUM WHEAT" },
            new() { HsCode = "1006.1000", Description = "RICE IN THE HUSK (PADDY OR ROUGH)" },
            new() { HsCode = "1701.1100", Description = "CANE SUGAR" },
            new() { HsCode = "2523.2900", Description = "PORTLAND CEMENT" },
            new() { HsCode = "2710.1290", Description = "PETROLEUM OILS AND OILS OBTAINED FROM BITUMINOUS MINERALS" },
            new() { HsCode = "2711.2100", Description = "NATURAL GAS" },
            new() { HsCode = "2716.0000", Description = "ELECTRICAL ENERGY" },
            new() { HsCode = "3002.1200", Description = "VACCINES FOR HUMAN MEDICINE" },
            new() { HsCode = "3004.9090", Description = "MEDICAMENTS" },
            new() { HsCode = "4011.1000", Description = "NEW PNEUMATIC TYRES OF RUBBER" },
            new() { HsCode = "5201.0000", Description = "COTTON, NOT CARDED OR COMBED" },
            new() { HsCode = "6109.1000", Description = "T-SHIRTS, SINGLETS AND OTHER VESTS OF COTTON" },
            new() { HsCode = "6204.6200", Description = "WOMEN'S TROUSERS AND BREECHES OF COTTON" },
            new() { HsCode = "7213.1000", Description = "BARS AND RODS OF IRON OR NON-ALLOY STEEL, HOT-ROLLED" },
            new() { HsCode = "8471.3000", Description = "PORTABLE AUTOMATIC DATA PROCESSING MACHINES" },
            new() { HsCode = "8517.1200", Description = "TELEPHONES FOR CELLULAR NETWORKS OR OTHER WIRELESS NETWORKS" },
            new() { HsCode = "8703.2319", Description = "MOTOR CARS CYLINDER CAPACITY EXCEEDING 1000CC BUT NOT EXCEEDING 1300CC" },
            new() { HsCode = "9403.6090", Description = "OTHER WOODEN FURNITURE" },
            new() { HsCode = "6110.2010", Description = "JERSEYS, PULLOVERS OF COTTON, KNITTED OR CROCHETED" },
            new() { HsCode = "8901.1000", Description = "CRUISE SHIPS, EXCURSION BOATS AND SIMILAR VESSELS" },
            new() { HsCode = "3808.9190", Description = "INSECTICIDES FOR RETAIL SALE" },
            new() { HsCode = "0901.1100", Description = "COFFEE NOT ROASTED, NOT DECAFFEINATED" },
            new() { HsCode = "2009.1100", Description = "ORANGE JUICE, FROZEN" },
            new() { HsCode = "8504.4090", Description = "STATIC CONVERTERS (OTHER)" },
            new() { HsCode = "9021.1000", Description = "ORTHOPAEDIC OR FRACTURE APPLIANCES" },
            new() { HsCode = "4901.9900", Description = "OTHER PRINTED BOOKS, BROCHURES, LEAFLETS" },
            new() { HsCode = "3401.1100", Description = "SOAP AND ORGANIC SURFACE-ACTIVE PRODUCTS FOR TOILET USE" },
            new() { HsCode = "8516.5000", Description = "MICROWAVE OVENS" }
        };
        return Task.FromResult(Result<List<HsCodeDto>>.Success(codes));
    }

    // ──────────────────────────────────────────────────────────────────────────
    // CHECK STATL (Active/In-Active based on NTN/CNIC length)
    // ──────────────────────────────────────────────────────────────────────────
    public async Task<Result<StatlResponseDto>> CheckStatlAsync(
        string bearerToken, string regno, string date, CancellationToken ct)
    {
        await Task.Delay(200, ct);

        var cleaned = regno.Trim().Replace("-", "");
        bool isActive = cleaned.Length == 7 || cleaned.Length == 13;

        return Result<StatlResponseDto>.Success(new StatlResponseDto
        {
            StatusCode = isActive ? "00" : "01",
            Status     = isActive ? "Active" : "In-Active"
        });
    }

    // ──────────────────────────────────────────────────────────────────────────
    // REGISTRATION TYPE (even last digit = Registered, odd = Unregistered)
    // ──────────────────────────────────────────────────────────────────────────
    public async Task<Result<RegistrationTypeResponseDto>> GetRegistrationTypeAsync(
        string bearerToken, string registrationNo, CancellationToken ct)
    {
        await Task.Delay(200, ct);

        var cleaned = registrationNo.Trim().Replace("-", "");
        bool isRegistered = cleaned.Length > 0 && (cleaned[^1] - '0') % 2 == 0;

        return Result<RegistrationTypeResponseDto>.Success(new RegistrationTypeResponseDto
        {
            StatusCode       = "00",
            RegistrationNo   = registrationNo,
            RegistrationType = isRegistered ? "Registered" : "Unregistered"
        });
    }
}
