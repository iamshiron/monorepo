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
	useQuery,
	useQueryClient,
} from "@tanstack/react-query";
import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { toast } from "sonner";
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
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
	Select,
	SelectContent,
	SelectItem,
	SelectTrigger,
	SelectValue,
} from "@/components/ui/select";
import { Spinner } from "@/components/ui/spinner";
import { useAuth } from "@/hooks/useAuth";
import { collectionApi, listsApi } from "@/lib/api";
import type {
	AddCharacterRequest,
	CollectionEntry,
	CollectionExportRequest,
} from "@/types";

export const Route = createFileRoute("/collection")({
	component: CollectionPage,
});

function useDebouncedValue<T>(value: T, delay: number): T {
	const [debouncedValue, setDebouncedValue] = useState(value);

	useEffect(() => {
		const timer = setTimeout(() => setDebouncedValue(value), delay);
		return () => clearTimeout(timer);
	}, [value, delay]);

	return debouncedValue;
}

const FILTERS_STORAGE_KEY = "mutils-collection-filters";

function usePersistedFilters(): [
	CollectionFilters,
	(filters: CollectionFilters) => void,
] {
	const [filters, setFilters] = useState<CollectionFilters>(() => {
		try {
			const stored = localStorage.getItem(FILTERS_STORAGE_KEY);
			if (stored) {
				const parsed = JSON.parse(stored);
				return { ...DEFAULT_FILTERS, ...parsed };
			}
		} catch (e) {
			console.error("Failed to parse stored filters:", e);
		}
		return DEFAULT_FILTERS;
	});

	const setPersistedFilters = (newFilters: CollectionFilters) => {
		localStorage.setItem(FILTERS_STORAGE_KEY, JSON.stringify(newFilters));
		setFilters(newFilters);
	};

	return [filters, setPersistedFilters];
}

function CollectionPage() {
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
	const [search, setSearch] = useState("");
	const [sortBy, setSortBy] = useState("rank");
	const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");
	const [page, setPage] = useState(1);
	const [filterSheetOpen, setFilterSheetOpen] = useState(false);
	const [filters, setFilters] = usePersistedFilters();
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
	const navigate = useNavigate();

	const debouncedSearch = useDebouncedValue(search, 300);

	const hasActiveFilters =
		minKeys > 0 ||
		minKakera > 0 ||
		disabledFilter !== "all" ||
		selectedKeyTypes.length > 0 ||
		wishStatus !== null ||
		isFavoriteFilter !== null ||
		selectedSeries.length > 0;

	const clearAllFilters = () => {
		setFilters(DEFAULT_FILTERS);
	};

	useEffect(() => {
		setPage(1);
	}, [
		debouncedSearch,
		minKeys,
		minKakera,
		disabledFilter,
		selectedKeyTypes,
		wishStatus,
		isFavoriteFilter,
		selectedSeries,
	]);

	const { data, isLoading, error } = useQuery({
		queryKey: [
			"collection",
			{
				search: debouncedSearch,
				sortBy,
				sortOrder,
				page,
				minKeys: minKeys || undefined,
				minKakera: minKakera || undefined,
				isDisabled:
					disabledFilter === "all" ? undefined : disabledFilter === "disabled",
				keyTypes:
					selectedKeyTypes.length > 0 ? selectedKeyTypes.join(",") : undefined,
				wishStatus: wishStatus ?? undefined,
				isFavorite: isFavoriteFilter ?? undefined,
				series:
					selectedSeries.length > 0 ? selectedSeries.join(",") : undefined,
			},
		],
		queryFn: () =>
			collectionApi.get({
				search: debouncedSearch,
				sortBy,
				sortOrder,
				page,
				pageSize: 60,
				minKeys: minKeys || undefined,
				minKakera: minKakera || undefined,
				isDisabled:
					disabledFilter === "all" ? undefined : disabledFilter === "disabled",
				keyTypes:
					selectedKeyTypes.length > 0 ? selectedKeyTypes.join(",") : undefined,
				wishStatus: wishStatus ?? undefined,
				isFavorite: isFavoriteFilter ?? undefined,
				series:
					selectedSeries.length > 0 ? selectedSeries.join(",") : undefined,
			}),
		enabled: isAuthenticated,
		placeholderData: keepPreviousData,
	});

	const { data: stats } = useQuery({
		queryKey: ["collection-stats"],
		queryFn: () => collectionApi.getStats(),
		enabled: isAuthenticated,
	});

	const { data: seriesList } = useQuery({
		queryKey: ["collection-series"],
		queryFn: () => collectionApi.getSeries(),
		enabled: isAuthenticated,
	});

	const { data: imageStatus, refetch: refetchImageStatus } = useQuery({
		queryKey: ["image-status"],
		queryFn: () => collectionApi.getImageStatus(),
		enabled: isAuthenticated,
		refetchInterval: (query) => {
			const d = query.state.data;
			return d && d.pending > 0 ? 5000 : false;
		},
	});

	const { data: wishlistData } = useQuery({
		queryKey: ["wishlist-all"],
		queryFn: () => listsApi.getWishlist({ pageSize: 1000 }),
		enabled: isAuthenticated,
	});

	const { data: wishlistStats } = useQuery({
		queryKey: ["wishlist-stats"],
		queryFn: () => listsApi.getWishlistStats(),
		enabled: isAuthenticated,
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
		}) => collectionApi.import(data, disabledCharacters),
		onSuccess: () => {
			queryClient.invalidateQueries({ queryKey: ["collection"] });
			queryClient.invalidateQueries({ queryKey: ["collection-stats"] });
			refetchImageStatus();
		},
	});

	const clearMutation = useMutation({
		mutationFn: collectionApi.clear,
		onSuccess: () => {
			queryClient.invalidateQueries({ queryKey: ["collection"] });
			queryClient.invalidateQueries({ queryKey: ["collection-stats"] });
		},
	});

	const processImagesMutation = useMutation({
		mutationFn: collectionApi.processImages,
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
		}) => collectionApi.update(id, data),
		onSuccess: () => {
			queryClient.invalidateQueries({ queryKey: ["collection"] });
		},
	});

	const deleteMutation = useMutation({
		mutationFn: (id: string) => collectionApi.delete(id),
		onSuccess: () => {
			queryClient.invalidateQueries({ queryKey: ["collection"] });
			queryClient.invalidateQueries({ queryKey: ["collection-stats"] });
		},
	});

	const importSeriesMutation = useMutation({
		mutationFn: (data: string) => collectionApi.importSeries(data),
		onSuccess: () => {
			queryClient.invalidateQueries({ queryKey: ["collection"] });
		},
	});

	const addCharacterMutation = useMutation({
		mutationFn: (request: AddCharacterRequest) =>
			collectionApi.addCharacter(request),
		onSuccess: () => {
			queryClient.invalidateQueries({ queryKey: ["collection"] });
			queryClient.invalidateQueries({ queryKey: ["collection-stats"] });
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
		}) =>
			listsApi.addToWishlist({
				characterId,
				isStarwish,
			}),
		onSuccess: () => {
			queryClient.invalidateQueries({ queryKey: ["wishlist"] });
			queryClient.invalidateQueries({ queryKey: ["wishlist-all"] });
			queryClient.invalidateQueries({ queryKey: ["wishlist-stats"] });
		},
	});

	const removeFromWishlistMutation = useMutation({
		mutationFn: (id: string) => listsApi.removeFromWishlist(id),
		onSuccess: () => {
			queryClient.invalidateQueries({ queryKey: ["wishlist"] });
			queryClient.invalidateQueries({ queryKey: ["wishlist-all"] });
			queryClient.invalidateQueries({ queryKey: ["wishlist-stats"] });
		},
	});

	const toggleFavoriteMutation = useMutation({
		mutationFn: (id: string) => collectionApi.toggleFavorite(id),
		onSuccess: () => {
			queryClient.invalidateQueries({ queryKey: ["collection"] });
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
				`Added ${entry.character.name} to ${isStarwish ? "starwish" : "wishlist"}`,
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

	const handleExport = async (request: CollectionExportRequest) => {
		const data = await collectionApi.export(request);
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
	};

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
								<span className="text-border">·</span>
								<span>
									{stats.disabledCount?.toLocaleString() ?? 0} disabled
								</span>
								<span className="text-border">·</span>
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
				(imageStatus.pending > 0 || imageStatus.processing > 0) && (
					<div className="glass rounded-lg px-4 py-3 flex items-center gap-3">
						<Spinner className="size-4 text-primary" />
						<span className="text-sm">
							Processing images: {imageStatus.stored}/{imageStatus.total} cached
							{imageStatus.pending > 0 && ` · ${imageStatus.pending} pending`}
						</span>
					</div>
				)}

			{imageStatus && imageStatus.failed > 0 && (
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
						value={search}
						onChange={(e) => setSearch(e.target.value)}
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

				<Select value={sortBy} onValueChange={setSortBy}>
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
					</SelectContent>
				</Select>

				<Button
					variant="outline"
					size="icon"
					className="h-10 w-10"
					onClick={() => setSortOrder(sortOrder === "asc" ? "desc" : "asc")}
				>
					<SortAscendingIcon
						size={18}
						className={`transition-transform ${sortOrder === "desc" ? "rotate-180" : ""}`}
					/>
				</Button>
			</div>

			<FilterSheet
				open={filterSheetOpen}
				onOpenChange={setFilterSheetOpen}
				filters={filters}
				onFiltersChange={setFilters}
				stats={stats}
				wishlistStats={wishlistStats}
				seriesList={seriesList}
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
					<div className="grid grid-cols-3 sm:grid-cols-4 md:grid-cols-5 lg:grid-cols-6 xl:grid-cols-7 2xl:grid-cols-8 gap-2">
						{data.items.map((entry) => (
							<CharacterCard
								key={entry.id}
								entry={entry}
								onEdit={setEditingEntry}
								onDelete={setDeletingEntry}
								onAddToWishlist={handleAddToWishlist}
								onRemoveFromWishlist={handleRemoveFromWishlist}
								onToggleFavorite={handleToggleFavorite}
								wishlistStatus={wishlistMap.get(entry.character.id) ?? null}
							/>
						))}
					</div>

					{data.totalPages > 1 && (
						<div className="flex items-center justify-center gap-2 pt-4">
							<Button
								variant="outline"
								size="sm"
								onClick={() => setPage((p) => Math.max(1, p - 1))}
								disabled={page === 1}
							>
								Previous
							</Button>
							<div className="flex items-center gap-1.5 px-3">
								<span className="text-sm font-medium">{page}</span>
								<span className="text-muted-foreground">/</span>
								<span className="text-sm text-muted-foreground">
									{data.totalPages}
								</span>
							</div>
							<Button
								variant="outline"
								size="sm"
								onClick={() => setPage((p) => Math.min(data.totalPages, p + 1))}
								disabled={page === data.totalPages}
							>
								Next
							</Button>
						</div>
					)}
				</>
			)}

			<ImportModal
				isOpen={showImport}
				onClose={() => setShowImport(false)}
				onImport={async (data, disabledCharacters) => {
					return importMutation.mutateAsync({ data, disabledCharacters });
				}}
				onClear={async () => {
					await clearMutation.mutateAsync();
				}}
			/>

			<ExportModal
				isOpen={showExport}
				onClose={() => setShowExport(false)}
				onExport={handleExport}
			/>

			<SeriesImportModal
				isOpen={showSeriesImport}
				onClose={() => setShowSeriesImport(false)}
				onImport={async (data) => {
					return importSeriesMutation.mutateAsync(data);
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
				seriesList={seriesList}
				onAdd={async (data) => {
					const result = await addCharacterMutation.mutateAsync(data);
					toast.success(`Added ${data.name} to collection`);
					if (result.imagesQueued > 0) {
						toast.info(`${result.imagesQueued} image(s) queued for download`);
					}
				}}
			/>
		</div>
	);
}
