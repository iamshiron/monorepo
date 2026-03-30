import {
	CaretDownIcon,
	CheckIcon,
	FolderOpenIcon,
	HeartIcon,
	KeyIcon,
	StarIcon,
	XIcon,
} from "@phosphor-icons/react";
import { useState } from "react";
import { Button } from "@shiron/ui/components/ui/button";
import { Input } from "@shiron/ui/components/ui/input";
import { Label } from "@shiron/ui/components/ui/label";
import {
	Popover,
	PopoverContent,
	PopoverTrigger,
} from "@shiron/ui/components/ui/popover";
import { Separator } from "@shiron/ui/components/ui/separator";
import {
	Sheet,
	SheetContent,
	SheetHeader,
	SheetTitle,
} from "@shiron/ui/components/ui/sheet";
import { Slider } from "@shiron/ui/components/ui/slider";
import type { CollectionStats, SeriesWithCount, WishlistStats } from "@/types";

export interface CollectionFilters {
	minKeys: number;
	minKakera: number;
	disabledFilter: "all" | "disabled" | "enabled";
	selectedKeyTypes: string[];
	wishStatus: "wish" | "starwish" | "inwishlist" | null;
	isFavoriteFilter: boolean | null;
	selectedSeries: string[];
}

export const DEFAULT_FILTERS: CollectionFilters = {
	minKeys: 0,
	minKakera: 0,
	disabledFilter: "all",
	selectedKeyTypes: [],
	wishStatus: null,
	isFavoriteFilter: null,
	selectedSeries: [],
};

const KEY_TYPE_CONFIG: Record<
	string,
	{ label: string; color: string; bgColor: string }
> = {
	bronzekey: {
		label: "Bronze",
		color: "text-amber-600",
		bgColor: "bg-amber-600/10 border-amber-600/30",
	},
	silverkey: {
		label: "Silver",
		color: "text-slate-400",
		bgColor: "bg-slate-400/10 border-slate-400/30",
	},
	goldkey: {
		label: "Gold",
		color: "text-yellow-500",
		bgColor: "bg-yellow-500/10 border-yellow-500/30",
	},
	chaoskey: {
		label: "Chaos",
		color: "text-purple-500",
		bgColor: "bg-purple-500/10 border-purple-500/30",
	},
	rubykey: {
		label: "Ruby",
		color: "text-rose-500",
		bgColor: "bg-rose-500/10 border-rose-500/30",
	},
	emeraldkey: {
		label: "Emerald",
		color: "text-emerald-500",
		bgColor: "bg-emerald-500/10 border-emerald-500/30",
	},
	sapphirekey: {
		label: "Sapphire",
		color: "text-sky-500",
		bgColor: "bg-sky-500/10 border-sky-500/30",
	},
};

export function FilterSheet({
	open,
	onOpenChange,
	filters,
	onFiltersChange,
	stats,
	wishlistStats,
	seriesList,
}: {
	open: boolean;
	onOpenChange: (open: boolean) => void;
	filters: CollectionFilters;
	onFiltersChange: (filters: CollectionFilters) => void;
	stats?: CollectionStats;
	wishlistStats?: WishlistStats;
	seriesList?: SeriesWithCount[];
}) {
	const [seriesSearch, setSeriesSearch] = useState("");

	const hasActiveFilters =
		filters.minKeys > 0 ||
		filters.minKakera > 0 ||
		filters.disabledFilter !== "all" ||
		filters.selectedKeyTypes.length > 0 ||
		filters.wishStatus !== null ||
		filters.isFavoriteFilter !== null ||
		filters.selectedSeries.length > 0;

	const toggleKeyType = (keyType: string) => {
		onFiltersChange({
			...filters,
			selectedKeyTypes: filters.selectedKeyTypes.includes(keyType)
				? filters.selectedKeyTypes.filter((k) => k !== keyType)
				: [...filters.selectedKeyTypes, keyType],
		});
	};

	const toggleSeries = (seriesName: string) => {
		onFiltersChange({
			...filters,
			selectedSeries: filters.selectedSeries.includes(seriesName)
				? filters.selectedSeries.filter((s) => s !== seriesName)
				: [...filters.selectedSeries, seriesName],
		});
	};

	const filteredSeriesList =
		seriesList?.filter((series) =>
			series.name.toLowerCase().includes(seriesSearch.toLowerCase()),
		) ?? [];

	return (
		<Sheet open={open} onOpenChange={onOpenChange}>
			<SheetContent
				side="right"
				className="w-[360px] sm:max-w-[400px] overflow-y-auto p-0"
			>
				<SheetHeader className="px-6 pt-6 pb-2">
					<SheetTitle className="text-lg">Filters</SheetTitle>
				</SheetHeader>
				<div className="space-y-6 px-6 pb-6">
					<div className="space-y-4">
						<h4 className="text-sm font-medium flex items-center gap-2">
							<KeyIcon size={16} />
							Keys
						</h4>
						<div className="space-y-3">
							{stats && Object.keys(stats.keyDistribution).length > 0 && (
								<div className="flex gap-1.5 flex-wrap">
									{Object.entries(stats.keyDistribution)
										.sort(([a], [b]) => {
											const order = [
												"bronzekey",
												"silverkey",
												"goldkey",
												"chaoskey",
											];
											return order.indexOf(a) - order.indexOf(b);
										})
										.map(([key, count]) => {
											const config = KEY_TYPE_CONFIG[key] || {
												label: key.replace("key", ""),
												color: "text-muted-foreground",
												bgColor: "bg-muted/50",
											};
											const isSelected = filters.selectedKeyTypes.includes(key);
											return (
												<button
													type="button"
													key={key}
													onClick={() => toggleKeyType(key)}
													className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-medium border transition-all ${
														isSelected
															? `${config.bgColor} ${config.color} ring-1 ring-current/20`
															: "bg-muted/30 text-muted-foreground hover:bg-muted/50"
													}`}
												>
													<KeyIcon
														size={12}
														weight="fill"
														className={config.color}
													/>
													<span>{config.label}</span>
													<span className="opacity-60">{count}</span>
												</button>
											);
										})}
								</div>
							)}
							<div className="space-y-2">
								<div className="flex items-center justify-between">
									<Label className="text-xs text-muted-foreground">
										Minimum Keys
									</Label>
									<span className="text-xs font-mono text-primary">
										{filters.minKeys}+
									</span>
								</div>
								<Slider
									value={[filters.minKeys]}
									onValueChange={([v]) =>
										onFiltersChange({
											...filters,
											minKeys: v,
										})
									}
									min={0}
									max={20}
									step={1}
									className="py-2"
								/>
							</div>
						</div>
					</div>

					<Separator />

					<div className="space-y-4">
						<h4 className="text-sm font-medium flex items-center gap-2">
							<StarIcon size={16} />
							Kakera
						</h4>
						<div className="space-y-3">
							<div className="space-y-2">
								<div className="flex items-center justify-between">
									<Label className="text-xs text-muted-foreground">
										Minimum Kakera
									</Label>
									<span className="text-xs font-mono text-primary">
										{filters.minKakera.toLocaleString()}+
									</span>
								</div>
								<Slider
									value={[filters.minKakera]}
									onValueChange={([v]) =>
										onFiltersChange({
											...filters,
											minKakera: v,
										})
									}
									min={0}
									max={10000}
									step={100}
									className="py-2"
								/>
							</div>
						</div>
					</div>

					<Separator />

					<div className="space-y-4">
						<h4 className="text-sm font-medium">Status</h4>
						<div className="flex gap-1.5">
							{(["all", "enabled", "disabled"] as const).map((status) => (
								<Button
									key={status}
									variant={
										filters.disabledFilter === status ? "default" : "outline"
									}
									size="sm"
									onClick={() =>
										onFiltersChange({
											...filters,
											disabledFilter: status,
										})
									}
									className="flex-1 text-xs capitalize"
								>
									{status}
								</Button>
							))}
						</div>
					</div>

					<Separator />

					{(wishlistStats?.totalCount ?? 0) > 0 && (
						<>
							<div className="space-y-4">
								<h4 className="text-sm font-medium flex items-center gap-2">
									<StarIcon size={16} />
									Wishlist
								</h4>
								<div className="flex gap-1.5 flex-wrap">
									<button
										type="button"
										onClick={() =>
											onFiltersChange({
												...filters,
												wishStatus:
													filters.wishStatus === "starwish" ? null : "starwish",
											})
										}
										className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-medium border transition-all ${
											filters.wishStatus === "starwish"
												? "bg-warning/10 text-warning ring-1 ring-warning/20"
												: "bg-muted/30 text-muted-foreground hover:bg-muted/50"
										}`}
									>
										<StarIcon
											size={12}
											weight="fill"
											className="text-warning"
										/>
										<span>Starwish</span>
										<span className="opacity-60">
											{wishlistStats?.starwishCount ?? 0}
										</span>
									</button>
									<button
										type="button"
										onClick={() =>
											onFiltersChange({
												...filters,
												wishStatus:
													filters.wishStatus === "wish" ? null : "wish",
											})
										}
										className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-medium border transition-all ${
											filters.wishStatus === "wish"
												? "bg-muted-foreground/10 text-muted-foreground ring-1 ring-muted-foreground/20"
												: "bg-muted/30 text-muted-foreground hover:bg-muted/50"
										}`}
									>
										<StarIcon size={12} />
										<span>Wish</span>
										<span className="opacity-60">
											{wishlistStats?.regularCount ?? 0}
										</span>
									</button>
								</div>
							</div>

							<Separator />
						</>
					)}

					<div className="space-y-4">
						<h4 className="text-sm font-medium flex items-center gap-2">
							<HeartIcon size={16} />
							Other
						</h4>
						<div className="flex gap-1.5 flex-wrap">
							<button
								type="button"
								onClick={() =>
									onFiltersChange({
										...filters,
										isFavoriteFilter:
											filters.isFavoriteFilter === true ? null : true,
									})
								}
								className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-medium border transition-all ${
									filters.isFavoriteFilter === true
										? "bg-destructive/10 text-destructive ring-1 ring-destructive/20"
										: "bg-muted/30 text-muted-foreground hover:bg-muted/50"
								}`}
							>
								<HeartIcon size={12} weight="fill" />
								<span>Favorites</span>
							</button>
						</div>
					</div>

					<Separator />

					<div className="space-y-4">
						<h4 className="text-sm font-medium flex items-center gap-2">
							<FolderOpenIcon size={16} />
							Series
						</h4>
						{seriesList && seriesList.length > 0 ? (
							<Popover>
								<PopoverTrigger asChild>
									<Button
										variant="outline"
										role="combobox"
										className="w-full justify-between"
									>
										{filters.selectedSeries.length > 0 ? (
											<span className="truncate">
												{filters.selectedSeries.length} series selected
											</span>
										) : (
											<span className="text-muted-foreground">
												Select series...
											</span>
										)}
										<CaretDownIcon
											size={16}
											className="ml-2 shrink-0 opacity-50"
										/>
									</Button>
								</PopoverTrigger>
								<PopoverContent
									className="p-0"
									align="start"
									style={{
										width: "var(--radix-popover-trigger-width)",
									}}
								>
									<div className="p-1">
										<Input
											placeholder="Search series..."
											value={seriesSearch}
											onChange={(e) => setSeriesSearch(e.target.value)}
											className="h-8"
										/>
									</div>
									<div
										className="max-h-64 overflow-y-auto border-t"
										onWheel={(e) => e.stopPropagation()}
									>
										{filteredSeriesList.length === 0 ? (
											<div className="py-6 text-center text-xs text-muted-foreground">
												No series found.
											</div>
										) : (
											filteredSeriesList.map((series) => (
												<button
													type="button"
													key={series.id}
													onClick={() => toggleSeries(series.name)}
													className="flex items-center justify-between px-2 py-1.5 text-xs cursor-pointer hover:bg-accent rounded-sm mx-1 w-full text-left"
												>
													<span className="truncate flex-1">{series.name}</span>
													<span className="text-muted-foreground shrink-0 ml-2">
														{series.characterCount}
													</span>
													{filters.selectedSeries.includes(series.name) && (
														<CheckIcon size={14} className="ml-2 shrink-0" />
													)}
												</button>
											))
										)}
									</div>
								</PopoverContent>
							</Popover>
						) : (
							<p className="text-sm text-muted-foreground">
								No series in your collection. Import series data to filter by
								series.
							</p>
						)}
						{filters.selectedSeries.length > 0 && (
							<div className="flex flex-wrap gap-1.5">
								{filters.selectedSeries.map((seriesName) => (
									<button
										type="button"
										key={seriesName}
										onClick={() => toggleSeries(seriesName)}
										className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs bg-secondary text-secondary-foreground hover:bg-secondary/80"
									>
										<span className="max-w-[120px] truncate">{seriesName}</span>
										<XIcon size={10} />
									</button>
								))}
							</div>
						)}
					</div>

					<Separator />

					{hasActiveFilters && (
						<Button
							variant="destructive"
							onClick={() => onFiltersChange(DEFAULT_FILTERS)}
							className="w-full"
						>
							<XIcon size={16} />
							Clear All Filters
						</Button>
					)}
				</div>
			</SheetContent>
		</Sheet>
	);
}

export { KEY_TYPE_CONFIG };
