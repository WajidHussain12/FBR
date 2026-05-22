namespace FBR_DI.Application.Interfaces;

public interface IQrCodeService
{
    string GenerateQrCodeBase64(string invoiceNumber);
}
