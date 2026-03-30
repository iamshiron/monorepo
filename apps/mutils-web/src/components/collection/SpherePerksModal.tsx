import { useQueryClient } from "@tanstack/react-query";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import { Button } from "@shiron/ui/components/ui/button";
import {
	Dialog,
	DialogContent,
	DialogDescription,
	DialogFooter,
	DialogHeader,
	DialogTitle,
} from "@shiron/ui/components/ui/dialog";
import {
	Select,
	SelectContent,
	SelectItem,
	SelectTrigger,
	SelectValue,
} from "@shiron/ui/components/ui/select";
import { Spinner } from "@shiron/ui/components/ui/spinner";
import { Switch } from "@shiron/ui/components/ui/switch";
import {
	getGetApiCollectionIdSpheresQueryKey,
	useGetApiCollectionIdSpheres,
	usePostApiCollectionIdSpheres,
} from "@/api/collection/collection";
import type { CollectionEntrySpherePerks } from "@/api/model";
import type { CollectionEntry } from "@/types";

const SPHERE_PERKS = [
	{
		key: "perk1" as const,
		id: 1,
		description:
			"Spawn chance increased for character(s) adjacent to this one in your $wishlist",
		costPerLevel: 200,
		isBoolean: false,
	},
	{
		key: "perk2" as const,
		id: 2,
		description: "Base kakera value increased",
		costPerLevel: 200,
		isBoolean: false,
	},
	{
		key: "perk3" as const,
		id: 3,
		description: "Chance to get +1 kakera button",
		costPerLevel: 200,
		isBoolean: false,
	},
	{
		key: "perk4" as const,
		id: 4,
		description: "Chance to get +1 key",
		costPerLevel: 200,
		isBoolean: false,
	},
	{
		key: "perk5" as const,
		id: 5,
		description:
			"Spheres earned per kakera button (except purple) clicked by you on this character",
		costPerLevel: 200,
		isBoolean: false,
	},
	{
		key: "perk6" as const,
		id: 6,
		description:
			"A random character from your wishlist might automatically appear after you roll this character (2% chance)",
		costPerLevel: 1000,
		isBoolean: true,
	},
	{
		key: "perk7" as const,
		id: 7,
		description:
			"Kakera buttons can turn into chaos kakera when you roll this character (1% chance per kakera except for red, light, dark and rainbow)",
		costPerLevel: 1000,
		isBoolean: true,
	},
	{
		key: "perk8" as const,
		id: 8,
		description:
			"A random kakera button appears when you roll this character for the first time that day",
		costPerLevel: 1000,
		isBoolean: true,
	},
	{
		key: "perk9" as const,
		id: 9,
		description:
			"1/7 to get 1 $oq per click. Up to 10 spheres clicked per day, only you can click",
		costPerLevel: 1000,
		isBoolean: true,
	},
	{
		key: "perk10" as const,
		id: 10,
		description:
			"The first $oh of the day generates +20 spheres and has +1% chance to give 1 $oq",
		costPerLevel: 1000,
		isBoolean: true,
	},
];

const EMPTY_PERKS: CollectionEntrySpherePerks = {
	perk1: 0,
	perk2: 0,
	perk3: 0,
	perk4: 0,
	perk5: 0,
	perk6: false,
	perk7: false,
	perk8: false,
	perk9: false,
	perk10: false,
};

function calculateTotalSpheres(perks: CollectionEntrySpherePerks): number {
	let total = 0;
	for (const perk of SPHERE_PERKS) {
		const value = perks[perk.key];
		if (perk.isBoolean) {
			if (value as boolean) {
				total += perk.costPerLevel;
			}
		} else {
			total += (value as number) * perk.costPerLevel;
		}
	}
	return total;
}

function LevelSelect({
	value,
	onChange,
	max,
}: {
	value: number;
	onChange: (v: number) => void;
	max: number;
}) {
	return (
		<Select value={String(value)} onValueChange={(v) => onChange(Number(v))}>
			<SelectTrigger className="w-20">
				<SelectValue />
			</SelectTrigger>
			<SelectContent>
				{Array.from({ length: max + 1 }, (_, i) => (
					<SelectItem key={String(i)} value={String(i)}>
						{i}
					</SelectItem>
				))}
			</SelectContent>
		</Select>
	);
}

export function SpherePerksModal({
	entry,
	isOpen,
	onClose,
}: {
	entry: CollectionEntry | null;
	isOpen: boolean;
	onClose: () => void;
}) {
	const queryClient = useQueryClient();

	const { data: serverPerks, isLoading } = useGetApiCollectionIdSpheres(
		entry?.id ?? "",
		{
			query: { enabled: isOpen && entry !== null },
		},
	);

	const [localPerks, setLocalPerks] =
		useState<CollectionEntrySpherePerks>(EMPTY_PERKS);

	useEffect(() => {
		if (serverPerks) {
			setLocalPerks(serverPerks);
		}
	}, [serverPerks]);

	useEffect(() => {
		if (!isOpen) {
			setLocalPerks(EMPTY_PERKS);
		}
	}, [isOpen]);

	const updatePerk = (
		key: keyof CollectionEntrySpherePerks,
		value: number | boolean,
	) => {
		setLocalPerks((prev) => ({ ...prev, [key]: value }));
	};

	const saveMutation = usePostApiCollectionIdSpheres({
		mutation: {
			onSuccess: async () => {
				await queryClient.invalidateQueries({
					// biome-ignore lint/style/noNonNullAssertion: guarded by early return above
					queryKey: getGetApiCollectionIdSpheresQueryKey(entry!.id),
				});
				toast.success("Sphere perks saved");
				onClose();
			},
			onError: () => {
				toast.error("Failed to save sphere perks");
			},
		},
	});

	const handleSave = () => {
		if (!entry) return;
		// biome-ignore lint/suspicious/noExplicitAny: type mismatch between API types
		saveMutation.mutate({ id: entry.id, data: localPerks as any });
	};

	const hasChanges =
		JSON.stringify(localPerks) !== JSON.stringify(serverPerks ?? EMPTY_PERKS);

	if (!entry) return null;

	return (
		<Dialog open={isOpen} onOpenChange={(open) => !open && onClose()}>
			<DialogContent className="sm:max-w-[75%] max-h-[85vh] overflow-y-auto">
				<DialogHeader>
					<DialogTitle>Edit Sphere Perks</DialogTitle>
					<DialogDescription>{entry.character.name}</DialogDescription>
				</DialogHeader>

				{isLoading ? (
					<div className="flex items-center justify-center py-8">
						<Spinner className="size-6" />
					</div>
				) : (
					<>
						<div className="flex items-center justify-between text-sm mb-4">
							<span className="text-muted-foreground">
								Total Spheres Invested
							</span>
							<span className="font-semibold">
								{calculateTotalSpheres(localPerks).toLocaleString()}
							</span>
						</div>

						<div className="grid grid-cols-1 md:grid-cols-2 md:grid-rows-5 md:grid-flow-col gap-3">
							{SPHERE_PERKS.map((perk) => (
								<div
									key={perk.key}
									className="flex items-center justify-between gap-4 rounded-md border p-3"
								>
									<div className="flex items-baseline gap-2 min-w-0 flex-1">
										<span className="text-muted-foreground font-mono text-sm shrink-0">
											[{perk.id}]
										</span>
										<span className="text-sm">{perk.description}</span>
									</div>
									{perk.isBoolean ? (
										<Switch
											checked={localPerks[perk.key] as boolean}
											onCheckedChange={(checked) =>
												updatePerk(perk.key, checked)
											}
										/>
									) : (
										<LevelSelect
											value={localPerks[perk.key] as number}
											onChange={(v) => updatePerk(perk.key, v)}
											max={5}
										/>
									)}
								</div>
							))}
						</div>
					</>
				)}

				<DialogFooter>
					<Button variant="outline" onClick={onClose}>
						Cancel
					</Button>
					<Button
						onClick={handleSave}
						disabled={!hasChanges || saveMutation.isPending || isLoading}
					>
						{saveMutation.isPending ? "Saving..." : "Save"}
					</Button>
				</DialogFooter>
			</DialogContent>
		</Dialog>
	);
}
