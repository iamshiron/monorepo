import { HeartIcon, KeyIcon, StarIcon, XIcon } from "@phosphor-icons/react";
import { Button } from "@shiron/ui/components/ui/button";
import type { CollectionFilters } from "./FilterSheet";
import { KEY_TYPE_CONFIG } from "./FilterSheet";

interface ActiveFiltersProps {
    filters: CollectionFilters;
    onFiltersChange: (filters: CollectionFilters) => void;
    onClearAll: () => void;
}

export function ActiveFilters({
    filters,
    onFiltersChange,
    onClearAll,
}: ActiveFiltersProps) {
    const {
        minKeys,
        minKakera,
        disabledFilter,
        selectedKeyTypes,
        wishStatus,
        isFavoriteFilter,
        selectedSeries,
    } = filters;

    const hasActiveFilters =
        minKeys > 0 ||
        minKakera > 0 ||
        disabledFilter !== "all" ||
        selectedKeyTypes.length > 0 ||
        wishStatus !== null ||
        isFavoriteFilter !== null ||
        selectedSeries.length > 0;

    if (!hasActiveFilters) return null;

    const toggleKeyType = (keyType: string) => {
        onFiltersChange({
            ...filters,
            selectedKeyTypes: selectedKeyTypes.filter((k) => k !== keyType),
        });
    };

    const toggleSeries = (seriesName: string) => {
        onFiltersChange({
            ...filters,
            selectedSeries: selectedSeries.filter((s) => s !== seriesName),
        });
    };

    return (
        <div className="flex items-center gap-2 flex-wrap">
            <span className="text-xs text-muted-foreground">
                Active filters:
            </span>
            {minKeys > 0 && (
                <button
                    type="button"
                    onClick={() => onFiltersChange({ ...filters, minKeys: 0 })}
                    className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs bg-secondary text-secondary-foreground hover:bg-secondary/80"
                >
                    {minKeys}+ keys
                    <XIcon size={10} />
                </button>
            )}
            {minKakera > 0 && (
                <button
                    type="button"
                    onClick={() =>
                        onFiltersChange({ ...filters, minKakera: 0 })
                    }
                    className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs bg-secondary text-secondary-foreground hover:bg-secondary/80"
                >
                    {minKakera.toLocaleString()}+ ka
                    <XIcon size={10} />
                </button>
            )}
            {disabledFilter !== "all" && (
                <button
                    type="button"
                    onClick={() =>
                        onFiltersChange({ ...filters, disabledFilter: "all" })
                    }
                    className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs bg-secondary text-secondary-foreground hover:bg-secondary/80 capitalize"
                >
                    {disabledFilter}
                    <XIcon size={10} />
                </button>
            )}
            {selectedKeyTypes.map((keyType) => {
                const config = KEY_TYPE_CONFIG[keyType];
                return (
                    <button
                        type="button"
                        key={keyType}
                        onClick={() => toggleKeyType(keyType)}
                        className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs bg-secondary text-secondary-foreground hover:bg-secondary/80"
                    >
                        <KeyIcon
                            size={10}
                            weight="fill"
                            className={config?.color}
                        />
                        {config?.label ?? keyType}
                        <XIcon size={10} />
                    </button>
                );
            })}
            {wishStatus && (
                <button
                    type="button"
                    onClick={() =>
                        onFiltersChange({ ...filters, wishStatus: null })
                    }
                    className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs bg-secondary text-secondary-foreground hover:bg-secondary/80"
                >
                    <StarIcon
                        size={10}
                        weight={wishStatus === "starwish" ? "fill" : "regular"}
                        className={
                            wishStatus === "starwish" ? "text-warning" : ""
                        }
                    />
                    {wishStatus === "starwish" ? "Starwish" : "Wish"}
                    <XIcon size={10} />
                </button>
            )}
            {isFavoriteFilter !== null && (
                <button
                    type="button"
                    onClick={() =>
                        onFiltersChange({ ...filters, isFavoriteFilter: null })
                    }
                    className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs bg-secondary text-secondary-foreground hover:bg-secondary/80"
                >
                    <HeartIcon
                        size={10}
                        weight="fill"
                        className="text-destructive"
                    />
                    Favorites
                    <XIcon size={10} />
                </button>
            )}
            {selectedSeries.map((seriesName) => (
                <button
                    type="button"
                    key={seriesName}
                    onClick={() => toggleSeries(seriesName)}
                    className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs bg-secondary text-secondary-foreground hover:bg-secondary/80"
                >
                    <span className="max-w-[100px] truncate">{seriesName}</span>
                    <XIcon size={10} />
                </button>
            ))}
            <Button
                variant="ghost"
                size="sm"
                onClick={onClearAll}
                className="h-6 px-2 text-xs text-muted-foreground hover:text-destructive"
            >
                Clear all
            </Button>
        </div>
    );
}
