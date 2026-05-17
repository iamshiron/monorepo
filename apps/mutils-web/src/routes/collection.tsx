import {
	DownloadIcon,
	FolderOpenIcon,
	FunnelIcon,
	ImagesIcon,
	ListBulletsIcon,
	MagnifyingGlassIcon,
	PlusIcon,
	SignInIcon,
	SortAscendingIcon,
	UploadIcon,
} from "@phosphor-icons/react";
import {
	keepPreviousData,
	useMutation,
	useQueryClient,
} from "@tanstack/react-query";
import {
	useDebouncedCallback,
	useDebouncedValue,
} from "@tanstack/react-pacer/debouncer";
import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { useCallback, useMemo, useRef, useState } from "react";
import { toast } from "sonner";
import {
	useGetApiCollection,
	useGetApiCollectionStats,
	useGetApiCollectionSeries,
	useGetImageStatus,
	getGetApiCollectionQueryKey,
	getGetApiCollectionStatsQueryKey,
	postApiCollectionImport,
	deleteApiCollectionClear,
	postApiCollectionProcessImages,
	putApiCollectionId,
	deleteApiCollectionId,
	postApiCollectionImportSeries,
	postApiCollectionAdd,
	postApiCollectionExport,
	postApiCollectionIdToggleFavorite,
} from "@/api/collection/collection";
import {
	useGetApiListsWishlist,
	useGetApiListsWishlistStats,
	getGetApiListsWishlistQueryKey,
	getGetApiListsWishlistStatsQueryKey,
	postApiListsWishlist,
	deleteApiListsWishlistId,
} from "@/api/lists-wishlist/lists-wishlist";
import { ActiveFilters } from "@/components/collection/ActiveFilters";
import { AddCharacterModal } from "@/components/collection/AddCharacterModal";
import { CharacterCard } from "@/components/collection/CharacterCard";
import { DeleteConfirmModal } from "@/components/collection/DeleteConfirmModal";
import { EditModal } from "@/components/collection/EditModal";
import { ExportModal } from "@/components/collection/ExportModal";
import {
	type CollectionFilters,
	DEFAULT_FILTERS,
	FilterSheet,
} from "@/components/collection/FilterSheet";
import { ImportModal } from "@/components/collection/ImportModal";
import { SeriesImportModal } from "@/components/collection/SeriesImportModal";
import { SpherePerksModal } from "@/components/collection/SpherePerksModal";
import { Button } from "@shiron/ui/components/ui/button";
import { Input } from "@shiron/ui/components/ui/input";
import {
	Pagination,
	PaginationContent,
	PaginationEllipsis,
	PaginationItem,
	PaginationLink,
	PaginationNext,
	PaginationPrevious,
} from "@shiron/ui/components/ui/pagination";
import {
	Select,
	SelectContent,
	SelectItem,
	SelectTrigger,
	SelectValue,
} from "@shiron/ui/components/ui/select";
import { Spinner } from "@shiron/ui/components/ui/spinner";
import { useAuth } from "@/hooks/useAuth";
import type { CollectionEntry } from "@/types";

interface CollectionSearchInput {
	search?: string;
	page?: number;
	sortBy?: string;
	sortOrder?: "asc" | "desc";
	minKeys?: number;
	minKakera?: number;
	disabledFilter?: "all" | "disabled" | "enabled";
	keyTypes?: string;
	wishStatus?: string;
	isFavorite?: boolean;
	series?: string;
}

interface ResolvedSearchParams {
	search: string;
	page: number;
	sortBy: string;
	sortOrder: "asc" | "desc";
	minKeys: number;
	minKakera: number;
	disabledFilter: "all" | "disabled" | "enabled";
	keyTypes: string;
	wishStatus: string;
	isFavorite: boolean;
	series: string;
}

const DEFAULT_SEARCH_PARAMS: ResolvedSearchParams = {
	search: "",
	page: 1,
	sortBy: "rank",
	sortOrder: "asc",
	minKeys: 0,
	minKakera: 0,
	disabledFilter: "all",
	keyTypes: "",
	wishStatus: "",
	isFavorite: false,
	series: "",
};

export const Route = createFileRoute("/collection")({
	component: CollectionPage,
	validateSearch: (search: Record<string, unknown>): CollectionSearchInput => {
		const result: CollectionSearchInput = {};
		if (typeof search.search === "string") result.search = search.search;
		if (search.page != null) result.page = Number(search.page) || 1;
		if (typeof search.sortBy === "string") result.sortBy = search.sortBy;
		if (search.sortOrder === "asc" || search.sortOrder === "desc")
			result.sortOrder = search.sortOrder;
		if (search.minKeys != null) result.minKeys = Number(search.minKeys) || 0;
		if (search.minKakera != null)
			result.minKakera = Number(search.minKakera) || 0;
		if (
			search.disabledFilter === "all" ||
			search.disabledFilter === "disabled" ||
			search.disabledFilter === "enabled"
		)
			result.disabledFilter = search.disabledFilter;
		if (typeof search.keyTypes === "string") result.keyTypes = search.keyTypes;
		if (typeof search.wishStatus === "string")
			result.wishStatus = search.wishStatus;
		if (search.isFavorite === "true") result.isFavorite = true;
		if (typeof search.series === "string") result.series = search.series;
		return result;
	},
});

function resolveSearchParams(raw: CollectionSearchInput): ResolvedSearchParams {
	return { ...DEFAULT_SEARCH_PARAMS, ...raw };
}

function stripDefaults(params: ResolvedSearchParams): CollectionSearchInput {
	const result: CollectionSearchInput = {};
	for (const key of Object.keys(
		DEFAULT_SEARCH_PARAMS,
	) as (keyof ResolvedSearchParams)[]) {
		if (params[key] !== DEFAULT_SEARCH_PARAMS[key]) {
			(result as Record<string, unknown>)[key] = params[key];
		}
	}
	return result;
}

function parseWishStatus(value: string): ("wish" | "starwish")[] {
	if (!value) return [];
	return value
		.split(",")
		.filter((v): v is "wish" | "starwish" => v === "wish" || v === "starwish");
}

function serializeWishStatus(arr: ("wish" | "starwish")[]): string {
	return arr.join(",");
}

function toWishStatusApiParam(
	arr: ("wish" | "starwish")[],
): string | undefined {
	if (arr.length === 0) return undefined;
	if (arr.includes("wish") && arr.includes("starwish")) return "inwishlist";
	return arr[0];
}

function generatePageNumbers(
	current: number,
	total: number,
	maxVisible = 5,
): (number | null)[] {
	if (total <= 1) return [1];

	const pages: (number | null)[] = [];

	if (total <= maxVisible + 2) {
		for (let i = 1; i <= total; i++) pages.push(i);
		return pages;
	}

	pages.push(1);

	const halfVisible = Math.floor(maxVisible / 2);
	let start = Math.max(2, current - halfVisible);
	const end = Math.min(total - 1, current + halfVisible);

	if (current <= halfVisible + 1) {
		const adjustedEnd = Math.min(total - 1, maxVisible);
		for (let i = 2; i <= adjustedEnd; i++) pages.push(i);
	} else if (current >= total - halfVisible) {
		const adjustedStart = Math.max(2, total - maxVisible + 1);
		if (adjustedStart > 2) pages.push(null);
		for (let i = adjustedStart; i <= total - 1; i++) pages.push(i);
	} else {
		if (start > 2) pages.push(null);
		for (let i = start; i <= end; i++) pages.push(i);
		if (end < total - 1) pages.push(null);
	}

	pages.push(total);

	return pages;
}

function urlParamsToFilters(params: ResolvedSearchParams): CollectionFilters {
	return {
		minKeys: params.minKeys,
		minKakera: params.minKakera,
		disabledFilter: params.disabledFilter as "all" | "disabled" | "enabled",
		selectedKeyTypes: params.keyTypes
			? params.keyTypes.split(",").filter(Boolean)
			: [],
		wishStatus: parseWishStatus(params.wishStatus),
		isFavoriteFilter: params.isFavorite,
		selectedSeries: params.series
			? params.series.split(",").filter(Boolean)
			: [],
	};
}

function CollectionPage() {
	const rawParams = Route.useSearch();
	const urlParams = useMemo(() => resolveSearchParams(rawParams), [rawParams]);
	const navigate = useNavigate();
	const gridRef = useRef<HTMLDivElement>(null);

	const [searchInput, setSearchInput] = useState(urlParams.search);
	const [showImport, setShowImport] = useState(false);
	const [showExport, setShowExport] = useState(false);
	const [showSeriesImport, setShowSeriesImport] = useState(false);
	const [showAddCharacter, setShowAddCharacter] = useState(false);
	const [editingEntry, setEditingEntry] = useState<CollectionEntry | null>(
		null,
	);
	const [deletingEntry, setDeletingEntry] = useState<CollectionEntry | null>(
		null,
	);
	const [editingSpherePerks, setEditingSpherePerks] =
		useState<CollectionEntry | null>(null);
	const [filterSheetOpen, setFilterSheetOpen] = useState(false);

	const filters = useMemo(() => urlParamsToFilters(urlParams), [urlParams]);

	const [debouncedSearch] = useDebouncedValue(urlParams.search, {
		wait: 300,
	});

	const {
		minKeys,
		minKakera,
		disabledFilter,
		selectedKeyTypes,
		wishStatus,
		isFavoriteFilter,
		selectedSeries,
	} = filters;

	const queryClient = useQueryClient();
	const { isAuthenticated, isLoading: authLoading } = useAuth();

	const navigateWith = useCallback(
		(updater: (current: ResolvedSearchParams) => ResolvedSearchParams) => {
			navigate({
				search: (current) => {
					const resolved = resolveSearchParams(
						current as CollectionSearchInput,
					);
					const next = updater(resolved);
					return stripDefaults(next) as never;
				},
				replace: true,
			});
		},
		[navigate],
	);

	const updateSearchUrl = useDebouncedCallback(
		(value: string) => {
			navigateWith((prev) => ({ ...prev, page: 1, search: value }));
		},
		{ wait: 300 },
	);

	const setParam = useCallback(
		(key: keyof ResolvedSearchParams, value: unknown) => {
			navigateWith((prev) => ({ ...prev, page: 1, [key]: value }));
		},
		[navigateWith],
	);

	const setFilters = useCallback(
		(newFilters: CollectionFilters) => {
			navigateWith((prev) => ({
				...prev,
				page: 1,
				minKeys: newFilters.minKeys,
				minKakera: newFilters.minKakera,
				disabledFilter: newFilters.disabledFilter,
				keyTypes: newFilters.selectedKeyTypes.join(","),
				wishStatus: serializeWishStatus(newFilters.wishStatus),
				isFavorite: newFilters.isFavoriteFilter,
				series: newFilters.selectedSeries.join(","),
			}));
		},
		[navigateWith],
	);

	const hasActiveFilters =
		minKeys > 0 ||
		minKakera > 0 ||
		disabledFilter !== "all" ||
		selectedKeyTypes.length > 0 ||
		wishStatus.length > 0 ||
		isFavoriteFilter ||
		selectedSeries.length > 0;

	const clearAllFilters = () => {
		setFilters(DEFAULT_FILTERS);
	};

	const wishStatusApiParam = toWishStatusApiParam(wishStatus);

	const { data, isLoading, error } = useGetApiCollection(
		{
			search: debouncedSearch,
			sortBy: urlParams.sortBy,
			sortOrder: urlParams.sortOrder,
			page: urlParams.page,
			pageSize: 60,
			minKeys: minKeys || undefined,
			minKakera: minKakera || undefined,
			isDisabled:
				disabledFilter === "all" ? undefined : disabledFilter === "disabled",
			keyTypes:
				selectedKeyTypes.length > 0 ? selectedKeyTypes.join(",") : undefined,
			wishStatus: wishStatusApiParam,
			isFavorite: isFavoriteFilter || undefined,
			series: selectedSeries.length > 0 ? selectedSeries.join(",") : undefined,
		},
		{
			query: {
				enabled: isAuthenticated,
				placeholderData: keepPreviousData,
			},
		},
	);

	const { data: stats } = useGetApiCollectionStats({
		query: { enabled: isAuthenticated },
	});

	const { data: seriesList } = useGetApiCollectionSeries({
		query: { enabled: isAuthenticated },
	});

	const { data: imageStatus, refetch: refetchImageStatus } = useGetImageStatus({
		query: {
			enabled: isAuthenticated,
			refetchInterval: (query) => {
				const d = query.state.data;
				return d && Number(d.pending) > 0 ? 5000 : false;
			},
		},
	});

	const { data: wishlistData } = useGetApiListsWishlist(
		{ pageSize: 1000 },
		{ query: { enabled: isAuthenticated } },
	);

	const { data: wishlistStats } = useGetApiListsWishlistStats({
		query: { enabled: isAuthenticated },
	});

	const wishlistMap = new Map<
		string,
		{ id: string; type: "wish" | "starwish" }
	>();
	if (wishlistData?.items) {
		for (const entry of wishlistData.items) {
			wishlistMap.set(entry.characterId, {
				id: entry.id,
				type: entry.isStarwish ? "starwish" : "wish",
			});
		}
	}

	const importMutation = useMutation({
		mutationFn: ({
			data,
			disabledCharacters,
		}: {
			data: string;
			disabledCharacters?: string;
		}) => postApiCollectionImport({ data, disabledCharacters }),
		onSuccess: () => {
			queryClient.invalidateQueries({
				queryKey: getGetApiCollectionQueryKey(),
			});
			queryClient.invalidateQueries({
				queryKey: getGetApiCollectionStatsQueryKey(),
			});
			refetchImageStatus();
		},
	});

	const clearMutation = useMutation({
		mutationFn: () => deleteApiCollectionClear(),
		onSuccess: () => {
			queryClient.invalidateQueries({
				queryKey: getGetApiCollectionQueryKey(),
			});
			queryClient.invalidateQueries({
				queryKey: getGetApiCollectionStatsQueryKey(),
			});
		},
	});

	const processImagesMutation = useMutation({
		mutationFn: () => postApiCollectionProcessImages(),
		onSuccess: () => {
			refetchImageStatus();
		},
	});

	const updateMutation = useMutation({
		mutationFn: ({
			id,
			...data
		}: {
			id: string;
			notes?: string;
			keyCount?: number;
		}) =>
			putApiCollectionId(id, {
				notes: data.notes ?? null,
				keyCount: data.keyCount,
			}),
		onSuccess: () => {
			queryClient.invalidateQueries({
				queryKey: getGetApiCollectionQueryKey(),
			});
		},
	});

	const deleteMutation = useMutation({
		mutationFn: (id: string) => deleteApiCollectionId(id),
		onSuccess: () => {
			queryClient.invalidateQueries({
				queryKey: getGetApiCollectionQueryKey(),
			});
			queryClient.invalidateQueries({
				queryKey: getGetApiCollectionStatsQueryKey(),
			});
		},
	});

	const importSeriesMutation = useMutation({
		mutationFn: (data: string) => postApiCollectionImportSeries({ data }),
		onSuccess: () => {
			queryClient.invalidateQueries({
				queryKey: getGetApiCollectionQueryKey(),
			});
		},
	});

	const addCharacterMutation = useMutation({
		mutationFn: (request: Parameters<typeof postApiCollectionAdd>[0]) =>
			postApiCollectionAdd(request),
		onSuccess: () => {
			queryClient.invalidateQueries({
				queryKey: getGetApiCollectionQueryKey(),
			});
			queryClient.invalidateQueries({
				queryKey: getGetApiCollectionStatsQueryKey(),
			});
			refetchImageStatus();
		},
	});

	const addToWishlistMutation = useMutation({
		mutationFn: ({
			characterId,
			isStarwish,
		}: {
			characterId: string;
			isStarwish: boolean;
		}) => postApiListsWishlist({ characterId, isStarwish }),
		onSuccess: () => {
			queryClient.invalidateQueries({
				queryKey: getGetApiListsWishlistQueryKey(),
			});
			queryClient.invalidateQueries({
				queryKey: getGetApiListsWishlistStatsQueryKey(),
			});
		},
	});

	const removeFromWishlistMutation = useMutation({
		mutationFn: (id: string) => deleteApiListsWishlistId(id),
		onSuccess: () => {
			queryClient.invalidateQueries({
				queryKey: getGetApiListsWishlistQueryKey(),
			});
			queryClient.invalidateQueries({
				queryKey: getGetApiListsWishlistStatsQueryKey(),
			});
		},
	});

	const toggleFavoriteMutation = useMutation({
		mutationFn: (id: string) => postApiCollectionIdToggleFavorite(id),
		onSuccess: () => {
			queryClient.invalidateQueries({
				queryKey: getGetApiCollectionQueryKey(),
			});
		},
	});

	const handleAddToWishlist = async (
		entry: CollectionEntry,
		isStarwish: boolean,
	) => {
		try {
			await addToWishlistMutation.mutateAsync({
				characterId: entry.character.id,
				isStarwish,
			});
			toast.success(
				`Added ${entry.character.name} to ${
					isStarwish ? "starwish" : "wishlist"
				}`,
			);
		} catch (error) {
			if (error instanceof Error) {
				toast.error(error.message);
			}
		}
	};

	const handleRemoveFromWishlist = async (wishlistEntryId: string) => {
		try {
			await removeFromWishlistMutation.mutateAsync(wishlistEntryId);
			toast.success("Removed from wishlist");
		} catch (error) {
			if (error instanceof Error) {
				toast.error(error.message);
			}
		}
	};

	const handleToggleFavorite = async (entry: CollectionEntry) => {
		try {
			const result = await toggleFavoriteMutation.mutateAsync(entry.id);
			toast.success(
				result.isFavorite
					? `Added ${entry.character.name} to favorites`
					: `Removed ${entry.character.name} from favorites`,
			);
		} catch (error) {
			if (error instanceof Error) {
				toast.error(error.message);
			}
		}
	};

	const handlePageChange = (newPage: number) => {
		navigateWith((prev) => ({ ...prev, page: newPage }));
		gridRef.current?.scrollIntoView({ behavior: "smooth" });
	};

	const totalPages = data ? Number(data.totalPages) : 0;
	const currentPage = urlParams.page;
	const pageNumbers = useMemo(
		() => generatePageNumbers(currentPage, totalPages),
		[currentPage, totalPages],
	);

	if (authLoading) {
		return (
			<div className="flex items-center justify-center min-h-[60vh]">
				<Spinner className="size-8 text-primary" />
			</div>
		);
	}

	if (!isAuthenticated) {
		return (
			<div className="flex flex-col items-center justify-center min-h-[60vh] text-center">
				<FolderOpenIcon size={48} className="text-muted-foreground/70 mb-4" />
				<h2 className="text-xl font-semibold mb-2">Login Required</h2>
				<p className="text-muted-foreground mb-4">
					Please login to view your collection
				</p>
				<Button
					onClick={() => navigate({ to: "/" })}
					className="h-9 px-6 text-sm"
				>
					<SignInIcon size={20} weight="bold" />
					Login with Discord
				</Button>
			</div>
		);
	}

	if (isLoading) {
		return (
			<div className="flex items-center justify-center min-h-[60vh]">
				<Spinner className="size-8 text-primary" />
			</div>
		);
	}

	if (error) {
		return (
			<div className="flex flex-col items-center justify-center min-h-[60vh] text-center">
				<p className="text-destructive mb-4">Failed to load collection</p>
				<Button onClick={() => window.location.reload()}>Retry</Button>
			</div>
		);
	}

	return (
		<div className="space-y-4">
			<div className="flex flex-col gap-4">
				<div className="flex flex-col sm:flex-row sm:items-start justify-between gap-4">
					<div>
						<h1 className="text-2xl font-bold tracking-tight">Collection</h1>
						{stats && (
							<div className="flex items-center gap-3 mt-1 text-sm text-muted-foreground">
								<span>{stats.totalCharacters.toLocaleString()} characters</span>
								<span className="text-border">&middot;</span>
								<span>
									{stats.disabledCount?.toLocaleString() ?? 0} disabled
								</span>
								<span className="text-border">&middot;</span>
								<span className="text-primary font-medium">
									{stats.totalKakera.toLocaleString()} ka
								</span>
							</div>
						)}
					</div>
					<div className="flex items-center gap-2">
						<Button
							variant="outline"
							size="sm"
							onClick={() => setShowExport(true)}
						>
							<DownloadIcon size={16} />
							Export
						</Button>
						{imageStatus &&
							imageStatus.pending === 0 &&
							imageStatus.stored < imageStatus.total && (
								<Button
									variant="outline"
									size="sm"
									onClick={() => processImagesMutation.mutate()}
									disabled={processImagesMutation.isPending}
								>
									<ImagesIcon size={16} />
									{processImagesMutation.isPending
										? "Caching..."
										: "Cache Images"}
								</Button>
							)}
						<Button
							variant="outline"
							size="sm"
							onClick={() => setShowSeriesImport(true)}
						>
							<ListBulletsIcon size={16} />
							Series
						</Button>
						<Button
							variant="outline"
							size="sm"
							onClick={() => setShowAddCharacter(true)}
						>
							<PlusIcon size={16} />
							Add Character
						</Button>
						<Button size="sm" onClick={() => setShowImport(true)}>
							<UploadIcon size={16} />
							Import
						</Button>
					</div>
				</div>
			</div>

			{imageStatus &&
				(Number(imageStatus.pending) > 0 ||
					Number(imageStatus.processing) > 0) && (
					<div className="glass rounded-lg px-4 py-3 flex items-center gap-3">
						<Spinner className="size-4 text-primary" />
						<span className="text-sm">
							Processing images: {imageStatus.stored}/{imageStatus.total} cached
							{Number(imageStatus.pending) > 0 &&
								` \u00b7 ${imageStatus.pending} pending`}
						</span>
					</div>
				)}

			{imageStatus && Number(imageStatus.failed) > 0 && (
				<div className="bg-destructive/10 border border-destructive/30 rounded-lg px-4 py-3 text-sm text-destructive">
					{imageStatus.failed} images failed to download
				</div>
			)}

			<div className="flex flex-col sm:flex-row gap-3">
				<div className="relative flex-1">
					<MagnifyingGlassIcon
						className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground/70"
						size={18}
					/>
					<Input
						type="text"
						value={searchInput}
						onChange={(e) => {
							setSearchInput(e.target.value);
							updateSearchUrl(e.target.value);
						}}
						placeholder="Search by name or series..."
						className="h-10 pl-10 pr-4"
					/>
				</div>

				<Button
					variant="outline"
					size="icon"
					className="h-10 w-10 relative"
					onClick={() => setFilterSheetOpen(true)}
				>
					<FunnelIcon size={18} />
					{hasActiveFilters && (
						<span className="absolute -top-1 -right-1 size-3 bg-primary rounded-full" />
					)}
				</Button>

				<Select
					value={urlParams.sortBy}
					onValueChange={(v) => setParam("sortBy", v)}
				>
					<SelectTrigger className="h-10! w-[160px]">
						<SelectValue />
					</SelectTrigger>
					<SelectContent>
						<SelectItem value="rank">Rank</SelectItem>
						<SelectItem value="name">Name</SelectItem>
						<SelectItem value="kakera">Kakera</SelectItem>
						<SelectItem value="user_kakera">Best Performing</SelectItem>
						<SelectItem value="claims">Claims</SelectItem>
						<SelectItem value="keys">Keys</SelectItem>
						<SelectItem value="spheres">Spheres</SelectItem>
					</SelectContent>
				</Select>

				<Button
					variant="outline"
					size="icon"
					className="h-10 w-10"
					onClick={() =>
						setParam(
							"sortOrder",
							urlParams.sortOrder === "asc" ? "desc" : "asc",
						)
					}
				>
					<SortAscendingIcon
						size={18}
						className={`transition-transform ${
							urlParams.sortOrder === "desc" ? "rotate-180" : ""
						}`}
					/>
				</Button>
			</div>

			<FilterSheet
				open={filterSheetOpen}
				onOpenChange={setFilterSheetOpen}
				filters={filters}
				onFiltersChange={setFilters}
				stats={stats as never}
				wishlistStats={wishlistStats as never}
				seriesList={seriesList as never}
			/>

			<ActiveFilters
				filters={filters}
				onFiltersChange={setFilters}
				onClearAll={clearAllFilters}
			/>

			{!data?.items.length ? (
				<div className="flex flex-col items-center justify-center py-20 text-center">
					<FolderOpenIcon size={48} className="text-muted-foreground/50 mb-4" />
					<h2 className="text-lg font-semibold mb-2">No characters found</h2>
					<p className="text-muted-foreground text-sm mb-6">
						{hasActiveFilters
							? "Try adjusting your filters"
							: "Import your collection from Mudae to get started"}
					</p>
					{hasActiveFilters ? (
						<Button variant="outline" onClick={clearAllFilters}>
							Clear Filters
						</Button>
					) : (
						<Button onClick={() => setShowImport(true)}>
							Import Collection
						</Button>
					)}
				</div>
			) : (
				<>
					<div
						ref={gridRef}
						className="grid grid-cols-3 sm:grid-cols-4 md:grid-cols-5 lg:grid-cols-6 xl:grid-cols-7 2xl:grid-cols-8 gap-2"
					>
						{data.items.map((entry) => (
							<CharacterCard
								key={entry.id}
								entry={entry as unknown as CollectionEntry}
								onEdit={setEditingEntry}
								onDelete={setDeletingEntry}
								onAddToWishlist={handleAddToWishlist}
								onRemoveFromWishlist={handleRemoveFromWishlist}
								onToggleFavorite={handleToggleFavorite}
								onEditSpherePerks={setEditingSpherePerks}
								wishlistStatus={wishlistMap.get(entry.character.id) ?? null}
							/>
						))}
					</div>

					{totalPages > 1 && (
						<Pagination className="pt-4">
							<PaginationContent>
								<PaginationItem>
									<PaginationPrevious
										onClick={() =>
											handlePageChange(Math.max(1, currentPage - 1))
										}
										className={
											currentPage === 1
												? "pointer-events-none opacity-50"
												: "cursor-pointer"
										}
									/>
								</PaginationItem>

								{pageNumbers.map((page, i) =>
									page === null ? (
										<PaginationItem key={`ellipsis-${i}`}>
											<PaginationEllipsis />
										</PaginationItem>
									) : (
										<PaginationItem key={page}>
											<PaginationLink
												onClick={() => handlePageChange(page)}
												isActive={page === currentPage}
												className="cursor-pointer"
											>
												{page}
											</PaginationLink>
										</PaginationItem>
									),
								)}

								<PaginationItem>
									<PaginationNext
										onClick={() =>
											handlePageChange(Math.min(totalPages, currentPage + 1))
										}
										className={
											currentPage === totalPages
												? "pointer-events-none opacity-50"
												: "cursor-pointer"
										}
									/>
								</PaginationItem>
							</PaginationContent>
						</Pagination>
					)}
				</>
			)}

			<ImportModal
				isOpen={showImport}
				onClose={() => setShowImport(false)}
				onImport={async (data, disabledCharacters) => {
					return importMutation.mutateAsync({
						data,
						disabledCharacters,
					}) as never;
				}}
				onClear={async () => {
					await clearMutation.mutateAsync();
				}}
			/>

			<ExportModal
				isOpen={showExport}
				onClose={() => setShowExport(false)}
				onExport={async (request) => {
					const data = await postApiCollectionExport(request);
					const json = JSON.stringify(data, null, 2);
					const blob = new Blob([json], { type: "application/json" });
					const url = URL.createObjectURL(blob);
					const a = document.createElement("a");
					a.href = url;
					a.download = "collection-export.json";
					document.body.appendChild(a);
					a.click();
					document.body.removeChild(a);
					URL.revokeObjectURL(url);
				}}
			/>

			<SeriesImportModal
				isOpen={showSeriesImport}
				onClose={() => setShowSeriesImport(false)}
				onImport={async (data) => {
					return importSeriesMutation.mutateAsync(data) as never;
				}}
			/>

			<EditModal
				entry={editingEntry}
				isOpen={editingEntry !== null}
				onClose={() => setEditingEntry(null)}
				onSave={async (id, data) => {
					await updateMutation.mutateAsync({ id, ...data });
				}}
			/>

			<DeleteConfirmModal
				entry={deletingEntry}
				isOpen={deletingEntry !== null}
				onClose={() => setDeletingEntry(null)}
				onConfirm={async (id) => {
					await deleteMutation.mutateAsync(id);
				}}
			/>

			<AddCharacterModal
				isOpen={showAddCharacter}
				onClose={() => setShowAddCharacter(false)}
				seriesList={seriesList as never}
				onAdd={async (data) => {
					const result = await addCharacterMutation.mutateAsync(data);
					toast.success(`Added ${data.name} to collection`);
					if (Number(result.imagesQueued) > 0) {
						toast.info(`${result.imagesQueued} image(s) queued for download`);
					}
				}}
			/>

			<SpherePerksModal
				isOpen={editingSpherePerks !== null}
				onClose={() => setEditingSpherePerks(null)}
				entry={editingSpherePerks}
			/>
		</div>
	);
}
