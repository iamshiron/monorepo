import { PlusIcon } from "@phosphor-icons/react";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import {
	Dialog,
	DialogContent,
	DialogFooter,
	DialogHeader,
	DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import type { AddCharacterRequest, SeriesWithCount } from "@/types";

interface AddCharacterModalProps {
	isOpen: boolean;
	onClose: () => void;
	onAdd: (data: AddCharacterRequest) => Promise<void>;
	seriesList?: SeriesWithCount[];
}

export function AddCharacterModal({
	isOpen,
	onClose,
	onAdd,
	seriesList,
}: AddCharacterModalProps) {
	const [name, setName] = useState("");
	const [rank, setRank] = useState<number | "">("");
	const [claims, setClaims] = useState<number | "">("");
	const [kakera, setKakera] = useState<number | "">("");
	const [sp, setSp] = useState<number | "">("");
	const [keyCount, setKeyCount] = useState<number | "">("");
	const [seriesCount, setSeriesCount] = useState<number | "">("");
	const [images, setImages] = useState<number | "">("");
	const [gifs, setGifs] = useState<number | "">("");
	const [imageUrl, setImageUrl] = useState("");
	const [seriesName, setSeriesName] = useState("");
	const [isLoading, setIsLoading] = useState(false);

	const handleSubmit = async () => {
		if (!name.trim()) return;

		setIsLoading(true);
		try {
			await onAdd({
				name: name.trim(),
				rank: rank === "" ? undefined : rank,
				claims: claims === "" ? undefined : claims,
				kakera: kakera === "" ? undefined : kakera,
				sp: sp === "" ? undefined : sp,
				keyCount: keyCount === "" ? undefined : keyCount,
				seriesCount: seriesCount === "" ? undefined : seriesCount,
				images: images === "" ? undefined : images,
				gifs: gifs === "" ? undefined : gifs,
				imageUrl: imageUrl.trim() || undefined,
				seriesName: seriesName.trim() || undefined,
			});
			handleClose();
		} finally {
			setIsLoading(false);
		}
	};

	const handleClose = () => {
		setName("");
		setRank("");
		setClaims("");
		setKakera("");
		setSp("");
		setKeyCount("");
		setSeriesCount("");
		setImages("");
		setGifs("");
		setImageUrl("");
		setSeriesName("");
		onClose();
	};

	return (
		<Dialog
			open={isOpen}
			onOpenChange={(open) => {
				if (!open) handleClose();
			}}
		>
			<DialogContent className="sm:max-w-lg max-h-[85vh] overflow-y-auto">
				<DialogHeader>
					<DialogTitle>Add Character</DialogTitle>
				</DialogHeader>

				<div className="space-y-4">
					<div>
						<Label htmlFor="add-name" className="mb-1.5">
							Name <span className="text-destructive">*</span>
						</Label>
						<Input
							id="add-name"
							value={name}
							onChange={(e) => setName(e.target.value)}
							placeholder="Character name"
							className="h-9"
						/>
					</div>

					<div className="grid grid-cols-2 gap-4">
						<div>
							<Label htmlFor="add-rank" className="mb-1.5">
								Rank
							</Label>
							<Input
								id="add-rank"
								type="number"
								value={rank}
								onChange={(e) =>
									setRank(e.target.value === "" ? "" : Number(e.target.value))
								}
								placeholder="e.g. 1234"
								className="h-9"
							/>
						</div>
						<div>
							<Label htmlFor="add-claims" className="mb-1.5">
								Claims
							</Label>
							<Input
								id="add-claims"
								type="number"
								value={claims}
								onChange={(e) =>
									setClaims(e.target.value === "" ? "" : Number(e.target.value))
								}
								placeholder="e.g. 50"
								className="h-9"
							/>
						</div>
					</div>

					<div className="grid grid-cols-2 gap-4">
						<div>
							<Label htmlFor="add-kakera" className="mb-1.5">
								Kakera
							</Label>
							<Input
								id="add-kakera"
								type="number"
								value={kakera}
								onChange={(e) =>
									setKakera(e.target.value === "" ? "" : Number(e.target.value))
								}
								placeholder="e.g. 500"
								className="h-9"
							/>
						</div>
						<div>
							<Label htmlFor="add-sp" className="mb-1.5">
								SP
							</Label>
							<Input
								id="add-sp"
								type="number"
								value={sp}
								onChange={(e) =>
									setSp(e.target.value === "" ? "" : Number(e.target.value))
								}
								placeholder="e.g. 10"
								className="h-9"
							/>
						</div>
					</div>

					<div className="grid grid-cols-2 gap-4">
						<div>
							<Label htmlFor="add-keycount" className="mb-1.5">
								Keys
							</Label>
							<Input
								id="add-keycount"
								type="number"
								value={keyCount}
								onChange={(e) =>
									setKeyCount(
										e.target.value === "" ? "" : Number(e.target.value),
									)
								}
								placeholder="e.g. 5"
								min={0}
								className="h-9"
							/>
							<p className="text-xs text-muted-foreground/70 mt-1">
								Bronze: 1-2, Silver: 3-5, Gold: 6-9, Chaos: 10+
							</p>
						</div>
						<div>
							<Label htmlFor="add-seriescount" className="mb-1.5">
								Series Count
							</Label>
							<Input
								id="add-seriescount"
								type="number"
								value={seriesCount}
								onChange={(e) =>
									setSeriesCount(
										e.target.value === "" ? "" : Number(e.target.value),
									)
								}
								placeholder="e.g. 3"
								min={1}
								className="h-9"
							/>
						</div>
					</div>

					<div className="grid grid-cols-2 gap-4">
						<div>
							<Label htmlFor="add-images" className="mb-1.5">
								Images
							</Label>
							<Input
								id="add-images"
								type="number"
								value={images}
								onChange={(e) =>
									setImages(e.target.value === "" ? "" : Number(e.target.value))
								}
								placeholder="e.g. 100"
								min={0}
								className="h-9"
							/>
						</div>
						<div>
							<Label htmlFor="add-gifs" className="mb-1.5">
								GIFs
							</Label>
							<Input
								id="add-gifs"
								type="number"
								value={gifs}
								onChange={(e) =>
									setGifs(e.target.value === "" ? "" : Number(e.target.value))
								}
								placeholder="e.g. 5"
								min={0}
								className="h-9"
							/>
						</div>
					</div>

					<div>
						<Label htmlFor="add-series" className="mb-1.5">
							Series
						</Label>
						<Input
							id="add-series"
							value={seriesName}
							onChange={(e) => setSeriesName(e.target.value)}
							placeholder="e.g. Genshin Impact"
							list="series-list"
							className="h-9"
						/>
						<datalist id="series-list">
							{seriesList?.map((s) => (
								<option key={s.id} value={s.name} />
							))}
						</datalist>
					</div>

					<div>
						<Label htmlFor="add-imageurl" className="mb-1.5">
							Image URL
						</Label>
						<Input
							id="add-imageurl"
							value={imageUrl}
							onChange={(e) => setImageUrl(e.target.value)}
							placeholder="https://..."
							className="h-9"
						/>
						<p className="text-xs text-muted-foreground/70 mt-1">
							Image will be downloaded and cached in the background
						</p>
					</div>
				</div>

				<DialogFooter>
					<Button variant="outline" onClick={handleClose}>
						Cancel
					</Button>
					<Button onClick={handleSubmit} disabled={isLoading || !name.trim()}>
						<PlusIcon size={18} />
						{isLoading ? "Adding..." : "Add Character"}
					</Button>
				</DialogFooter>
			</DialogContent>
		</Dialog>
	);
}
