namespace Shiron.Mutils.Core.DTOs;

public sealed record CollectionEntryDto(
    Guid Id,
    CharacterDto Character,
    DateTime? AcquiredAt,
    string? Notes,
    bool IsDisabled = false,
    bool IsFavorite = false
);

public sealed record CharacterDto(
    Guid Id,
    string Name,
    int? Rank,
    int? Claims,
    int? Images,
    int? Gifs,
    int? SeriesCount,
    string? KeyType,
    int? KeyCount,
    int? Kakera,
    int? Sp,
    string? ImageUrl,
    Guid? StoredImageId,
    string? SeriesName = null,
    CharacterKakeraStatsDto? KakeraStats = null
);

public sealed record CharacterKakeraStatsDto(
    int TotalValue,
    int TotalCount,
    Dictionary<string, int> ByType
);

public sealed record PaginatedResponse<T>(
    IReadOnlyList<T> Items,
    int Total,
    int Page,
    int PageSize,
    int TotalPages
);

public sealed record ImportRequest(string Data, string? DisabledCharacters = null);

public sealed record ImportResponse(
    int Imported,
    int Skipped,
    int Updated,
    IReadOnlyList<string> Errors,
    int ImagesQueued = 0,
    int? DisabledImported = null
);

public sealed record CollectionStatsDto(
    int TotalCharacters,
    int TotalKakera,
    Dictionary<string, int> KeyDistribution,
    int DisabledCount = 0
);

public sealed record ImportSeriesRequest(string Data);

public sealed record ImportSeriesResponse(
    int Updated,
    int NotFound,
    IReadOnlyList<string> NotFoundNames
);

public sealed record CollectionExportItemDto(
    string Name,
    int? Kakera,
    int? KeyCount,
    int? Sp,
    bool IsDisabled
);

public sealed record CollectionExportResponse(
    int TotalCount,
    int ExportedCount,
    IReadOnlyList<CollectionExportItemDto> Items
);

public sealed record CollectionExportRequest(
    int? MinKeys = null,
    string? SortBy = "kakera",
    string? SortOrder = "desc",
    int? Limit = null,
    bool? ExcludeDisabled = null
);

public sealed record SeriesWithCountDto(
    Guid Id,
    string Name,
    int CharacterCount
);

public sealed record AddCharacterRequest(
    string Name,
    int? Rank = null,
    int? Claims = null,
    int? Images = null,
    int? Gifs = null,
    int? SeriesCount = null,
    int? KeyCount = null,
    int? Kakera = null,
    int? Sp = null,
    string? ImageUrl = null,
    string? SeriesName = null
);

public sealed record AddCharacterResponse(
    Guid Id,
    Guid CharacterId,
    bool IsNewCharacter,
    int ImagesQueued
);

public sealed record CollectionEntrySpherePerks(
    int Perk1,
    int Perk2,
    int Perk3,
    int Perk4,
    int Perk5,
    bool Perk6,
    bool Perk7,
    bool Perk8,
    bool Perk9,
    bool Perk10
) {
    public static CollectionEntrySpherePerks Empty = new(0, 0, 0, 0, 0, false, false, false, false, false);
}
