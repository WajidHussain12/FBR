using FBR_DI.Application.Interfaces;
using QRCoder;

namespace FBR_DI.Infrastructure.Services;

public class QrCodeService : IQrCodeService
{
    public string GenerateQrCodeBase64(string invoiceNumber)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(invoiceNumber, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeBytes = qrCode.GetGraphic(10);
        return Convert.ToBase64String(qrCodeBytes);
    }
}
