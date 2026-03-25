import {
	CaretDownIcon,
	HeartIcon,
	KeyIcon,
	PencilIcon,
	StarIcon,
	TrashIcon,
	XIcon,
} from "@phosphor-icons/react";
import { memo } from "react";
import { Badge } from "@/components/ui/badge";
import {
	ContextMenu,
	ContextMenuContent,
	ContextMenuItem,
	ContextMenuSeparator,
	ContextMenuTrigger,
} from "@/components/ui/context-menu";
import {
	Tooltip,
	TooltipContent,
	TooltipTrigger,
} from "@/components/ui/tooltip";
import type { CollectionEntry } from "@/types";

const KEY_COLORS: Record<string, string> = {
	bronzekey: "text-warning",
	silverkey: "text-muted-foreground",
	goldkey: "text-warning",
	chaoskey: "text-chart-3",
	rubykey: "text-destructive",
	emeraldkey: "text-success",
	sapphirekey: "text-info",
};

export const CharacterCard = memo(function CharacterCard({
	entry,
	onEdit,
	onDelete,
	onAddToWishlist,
	onRemoveFromWishlist,
	onToggleFavorite,
	wishlistStatus,
}: {
	entry: CollectionEntry;
	onEdit: (entry: CollectionEntry) => void;
	onDelete: (entry: CollectionEntry) => void;
	onAddToWishlist: (entry: CollectionEntry, isStarwish: boolean) => void;
	onRemoveFromWishlist: (wishlistEntryId: string) => void;
	onToggleFavorite: (entry: CollectionEntry) => void;
	wishlistStatus?: { id: string; type: "wish" | "starwish" } | null;
}) {
	const character = entry.character;

	const imageSrc = character.storedImageId
		? `/api/collection/images/${character.storedImageId}`
		: character.imageUrl;

	const isDisabled = entry.isDisabled;

	return (
		<ContextMenu>
			<ContextMenuTrigger asChild>
				<div
					className={`glass rounded-lg p-2.5 lantern-top hover:shadow-lg transition-all group relative ${isDisabled ? "ring-2 ring-destructive/50 bg-destructive/5" : ""}`}
				>
					{isDisabled && (
						<Badge
							variant="destructive"
							className="absolute top-1.5 right-1.5 z-10 text-[10px] px-1.5 py-0"
						>
							Disabled
						</Badge>
					)}
					{entry.isFavorite && (
						<Tooltip>
							<TooltipTrigger asChild>
								<div className="absolute top-1.5 left-1.5 z-10">
									<HeartIcon
										size={14}
										weight="fill"
										className="text-destructive"
									/>
								</div>
							</TooltipTrigger>
							<TooltipContent>Favorite</TooltipContent>
						</Tooltip>
					)}
					{wishlistStatus && (
						<Tooltip>
							<TooltipTrigger asChild>
								<div
									className={`absolute z-10 ${entry.isFavorite ? "top-5 left-1.5" : "top-1.5 left-1.5"}`}
								>
									<StarIcon
										size={14}
										weight={
											wishlistStatus.type === "starwish" ? "fill" : "regular"
										}
										className={
											wishlistStatus.type === "starwish"
												? "text-warning"
												: "text-muted-foreground"
										}
									/>
								</div>
							</TooltipTrigger>
							<TooltipContent>
								{wishlistStatus.type === "starwish" ? "Starwish" : "Wishlist"}
							</TooltipContent>
						</Tooltip>
					)}
					<div className="aspect-square bg-muted rounded-md mb-2 flex items-center justify-center overflow-hidden relative">
						{imageSrc ? (
							<img
								src={imageSrc}
								alt={character.name}
								className={`w-full h-full object-cover rounded-md group-hover:scale-105 transition-transform ${isDisabled ? "opacity-60" : ""}`}
								loading="lazy"
							/>
						) : (
							<span className="text-muted-foreground/70 text-3xl">?</span>
						)}
					</div>
					<div className="flex items-start justify-between gap-1.5">
						<h3
							className={`font-medium text-sm truncate ${isDisabled ? "text-destructive" : ""}`}
							title={character.name}
						>
							{character.name}
						</h3>
						{character.keyType && (
							<div className="flex items-center gap-0.5 shrink-0">
								<KeyIcon
									size={12}
									className={
										KEY_COLORS[character.keyType] || "text-muted-foreground/70"
									}
									weight="fill"
								/>
								{character.keyCount && (
									<span
										className={`text-[10px] ${KEY_COLORS[character.keyType] || "text-muted-foreground/70"}`}
									>
										×{character.keyCount}
									</span>
								)}
							</div>
						)}
					</div>
					{character.seriesName && (
						<p
							className="text-[10px] text-muted-foreground/70 truncate"
							title={character.seriesName}
						>
							{character.seriesName}
						</p>
					)}
					<div className="flex items-center justify-between mt-1.5 text-xs">
						<span className="text-muted-foreground">
							#{character.rank ?? "?"}
						</span>
						<div className="flex items-center gap-1.5">
							{character.sp && (
								<span className="text-info">
									{character.sp.toLocaleString()}sp
								</span>
							)}
							<span className="text-primary">
								{character.kakera?.toLocaleString() ?? "?"}ka
							</span>
						</div>
					</div>
					{character.claims !== null && (
						<p className="text-[10px] text-muted-foreground/70 mt-0.5">
							{character.claims} claims
							{character.images !== null && ` · ${character.images} img`}
							{character.gifs !== null && ` + ${character.gifs} gif`}
						</p>
					)}

					{character.kakeraStats && character.kakeraStats.totalValue > 0 && (
						<div className="relative mt-1.5 pt-1.5 border-t border-border/50">
							<div className="flex items-center justify-between text-[10px]">
								<span className="text-muted-foreground">User Kakera</span>
								<Tooltip>
									<TooltipTrigger asChild>
										<span className="flex items-center gap-0.5 cursor-default hover:text-primary transition-colors font-bold text-primary">
											<span>
												{character.kakeraStats.totalValue.toLocaleString()}
											</span>
											<CaretDownIcon
												size={10}
												weight="bold"
												className="opacity-50"
											/>
										</span>
									</TooltipTrigger>
									<TooltipContent
										side="top"
										className="glass p-2 min-w-[120px]"
									>
										<p className="text-[9px] uppercase tracking-wider text-muted-foreground mb-1 border-b border-border/30 pb-0.5 font-bold text-center">
											Breakdown
										</p>
										<div className="space-y-0.5">
											{Object.entries(character.kakeraStats.byType).map(
												([type, value]) => (
													<div
														key={type}
														className="flex justify-between items-center gap-3"
													>
														<span className="capitalize text-[10px]">
															{type}
														</span>
														<span className="font-mono text-primary font-bold text-[10px]">
															{value.toLocaleString()}
														</span>
													</div>
												),
											)}
										</div>
									</TooltipContent>
								</Tooltip>
							</div>
						</div>
					)}
				</div>
			</ContextMenuTrigger>
			<ContextMenuContent>
				<ContextMenuItem onClick={() => onToggleFavorite(entry)}>
					<HeartIcon
						size={14}
						className="mr-2"
						weight={entry.isFavorite ? "fill" : "regular"}
					/>
					{entry.isFavorite ? "Remove from Favorites" : "Add to Favorites"}
				</ContextMenuItem>
				<ContextMenuSeparator />
				{wishlistStatus ? (
					<ContextMenuItem
						onClick={() => onRemoveFromWishlist(wishlistStatus.id)}
						className="text-destructive focus:text-destructive"
					>
						<XIcon size={14} className="mr-2" />
						Remove from{" "}
						{wishlistStatus.type === "starwish" ? "Starwish" : "Wishlist"}
					</ContextMenuItem>
				) : (
					<>
						<ContextMenuItem onClick={() => onAddToWishlist(entry, false)}>
							<StarIcon size={14} className="mr-2" />
							Add to Wishlist
						</ContextMenuItem>
						<ContextMenuItem onClick={() => onAddToWishlist(entry, true)}>
							<StarIcon size={14} className="mr-2 text-warning" weight="fill" />
							Add as Starwish
						</ContextMenuItem>
					</>
				)}
				<ContextMenuSeparator />
				<ContextMenuItem onClick={() => onEdit(entry)}>
					<PencilIcon size={14} className="mr-2" />
					Edit
				</ContextMenuItem>
				<ContextMenuItem
					onClick={() => onDelete(entry)}
					className="text-destructive focus:text-destructive"
				>
					<TrashIcon size={14} className="mr-2" />
					Remove
				</ContextMenuItem>
			</ContextMenuContent>
		</ContextMenu>
	);
});
