import { DownloadIcon } from "@phosphor-icons/react";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import {
	Dialog,
	DialogContent,
	DialogFooter,
	DialogHeader,
	DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
	Select,
	SelectContent,
	SelectItem,
	SelectTrigger,
	SelectValue,
} from "@/components/ui/select";
import type { CollectionExportRequest } from "@/types";

export function ExportModal({
	isOpen,
	onClose,
	onExport,
}: {
	isOpen: boolean;
	onClose: () => void;
	onExport: (request: CollectionExportRequest) => Promise<void>;
}) {
	const [minKeys, setMinKeys] = useState<number | "">("");
	const [sortBy, setSortBy] =
		useState<CollectionExportRequest["sortBy"]>("kakera");
	const [sortOrder, setSortOrder] =
		useState<CollectionExportRequest["sortOrder"]>("desc");
	const [limit, setLimit] = useState<number | "">("");
	const [excludeDisabled, setExcludeDisabled] = useState(false);
	const [isExporting, setIsExporting] = useState(false);

	const handleExport = async () => {
		setIsExporting(true);
		try {
			await onExport({
				minKeys: minKeys === "" ? undefined : minKeys,
				sortBy,
				sortOrder,
				limit: limit === "" ? undefined : limit,
				excludeDisabled,
			});
			onClose();
		} finally {
			setIsExporting(false);
		}
	};

	return (
		<Dialog
			open={isOpen}
			onOpenChange={(open) => {
				if (!open) onClose();
			}}
		>
			<DialogContent className="sm:max-w-md">
				<DialogHeader>
					<DialogTitle>Export Collection</DialogTitle>
				</DialogHeader>

				<div className="space-y-4">
					<div>
						<Label htmlFor="minKeys" className="mb-1">
							Minimum Keys
						</Label>
						<Input
							id="minKeys"
							type="number"
							value={minKeys}
							onChange={(e) =>
								setMinKeys(e.target.value === "" ? "" : Number(e.target.value))
							}
							placeholder="Any"
							min={0}
							className="h-9"
						/>
					</div>

					<div>
						<Label htmlFor="sortBy" className="mb-1">
							Sort By
						</Label>
						<Select
							value={sortBy}
							onValueChange={(value) =>
								setSortBy(value as CollectionExportRequest["sortBy"])
							}
						>
							<SelectTrigger className="w-full">
								<SelectValue />
							</SelectTrigger>
							<SelectContent>
								<SelectItem value="kakera">Kakera Value</SelectItem>
								<SelectItem value="keyCount">Key Count</SelectItem>
								<SelectItem value="sp">Spheres</SelectItem>
								<SelectItem value="name">Name</SelectItem>
							</SelectContent>
						</Select>
					</div>

					<div>
						<Label htmlFor="sortOrder" className="mb-1">
							Sort Order
						</Label>
						<Select
							value={sortOrder}
							onValueChange={(value) =>
								setSortOrder(value as CollectionExportRequest["sortOrder"])
							}
						>
							<SelectTrigger className="w-full">
								<SelectValue />
							</SelectTrigger>
							<SelectContent>
								<SelectItem value="desc">Descending</SelectItem>
								<SelectItem value="asc">Ascending</SelectItem>
							</SelectContent>
						</Select>
					</div>

					<div>
						<Label htmlFor="limitResults" className="mb-1">
							Limit Results
						</Label>
						<Input
							id="limitResults"
							type="number"
							value={limit}
							onChange={(e) =>
								setLimit(e.target.value === "" ? "" : Number(e.target.value))
							}
							placeholder="All"
							min={1}
							className="h-9"
						/>
					</div>

					<div className="flex items-center gap-2">
						<Checkbox
							id="exclude-disabled"
							checked={excludeDisabled}
							onCheckedChange={(checked) =>
								setExcludeDisabled(checked as boolean)
							}
						/>
						<Label htmlFor="exclude-disabled">
							Exclude disabled characters
						</Label>
					</div>
				</div>

				<DialogFooter>
					<Button variant="outline" onClick={onClose}>
						Cancel
					</Button>
					<Button
						onClick={handleExport}
						disabled={isExporting}
						className="h-9 px-4 text-sm"
					>
						<DownloadIcon size={18} />
						{isExporting ? "Exporting..." : "Export JSON"}
					</Button>
				</DialogFooter>
			</DialogContent>
		</Dialog>
	);
}
