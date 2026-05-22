using FBR_DI.Domain.Enums;

namespace FBR_DI.Infrastructure.SeedData;

public static class FbrSaleTypeMapper
{
    public static readonly Dictionary<SaleType, string> SaleTypeToString = new()
    {
        { SaleType.GoodsAtStandardRate,          "Goods at standard rate (default)" },
        { SaleType.GoodsAtReducedRate,           "Goods at reduced rate" },
        { SaleType.ExemptGoods,                  "Exempt Goods" },
        { SaleType.GoodsAtZeroRate,              "Goods at zero-rate" },
        { SaleType.ThirdScheduleGoods,           "3rd Schedule Goods" },
        { SaleType.CottonGinners,                "Cotton Ginners" },
        { SaleType.TelecommunicationServices,    "Telecommunication services" },
        { SaleType.TollManufacturing,            "Toll Manufacturing" },
        { SaleType.PetroleumProducts,            "Petroleum Products" },
        { SaleType.ElectricitySupplyToRetailers, "Electricity Supply to Retailers" },
        { SaleType.GasToCNGStations,             "Gas to CNG Stations" },
        { SaleType.MobilePhones,                 "Mobile Phones" },
        { SaleType.ProcessingConversionOfGoods,  "Processing/Conversion of Goods" },
        { SaleType.GoodsFEDInSTMode,             "Goods (FED in ST Mode)" },
        { SaleType.ServicesFEDInSTMode,          "Services (FED in ST Mode)" },
        { SaleType.Services,                     "Services" },
        { SaleType.ElectricVehicle,              "Electric Vehicle" },
        { SaleType.CementConcreteBlock,          "Cement/Concrete Block" },
        { SaleType.PotassiumChlorate,            "Potassium Chlorate" },
        { SaleType.CNGSales,                     "CNG Sales" },
        { SaleType.GoodsAsPerSRO297,             "Goods as per SRO 297" },
        { SaleType.NonAdjustableSupplies,        "Non-Adjustable Supplies" },
        { SaleType.SteelMeltingAndReRolling,     "Steel Melting and Re-Rolling" },
        { SaleType.ShipBreaking,                 "Ship Breaking" }
    };

    public static string GetString(SaleType saleType)
        => SaleTypeToString.TryGetValue(saleType, out var value) ? value : saleType.ToString();

    public static IEnumerable<string> GetAllStrings() => SaleTypeToString.Values;
}
