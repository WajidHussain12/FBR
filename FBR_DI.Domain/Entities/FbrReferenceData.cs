using FBR_DI.Domain.Common;

namespace FBR_DI.Domain.Entities;

public class FbrReferenceData : BaseEntity
{
    public string ReferenceType { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ParentCode { get; set; }
    public int SortOrder { get; set; } = 0;
    public DateTime LastSyncedAt { get; set; }
}
