import { Card, CardContent } from "@shiron/ui/components/ui/card";
import { Input } from "@shiron/ui/components/ui/input";
import {
	ToggleGroup,
	ToggleGroupItem,
} from "@shiron/ui/components/ui/toggle-group";
import { useMemo, useState } from "react";
import type { Attribute } from "@/api/model/attribute";
import type { CharacterDTO } from "@/api/model/characterDTO";
import {
	ATTRIBUTE_COLOR,
	ATTRIBUTE_ENTRIES,
	getAttributeName,
} from "@/types/attribute";

interface CharacterSelectorProps {
	characters: CharacterDTO[];
	onSelect?: (id: string | number) => void;
	selectedId?: string | number | null;
}

export function CharacterSelector({
	characters: all,
	onSelect,
	selectedId: externalSelectedId,
}: CharacterSelectorProps) {
	const [nameFilter, setNameFilter] = useState("");
	const [attributeFilter, setAttributeFilter] = useState<Attribute | null>(
		null,
	);
	const [internalSelectedId, setInternalSelectedId] = useState<
		string | number | null
	>(null);

	const selectedId = externalSelectedId ?? internalSelectedId;

	const { characters, attributeCounts } = useMemo(() => {
		const counts = new Map<Attribute, number>();
		for (const c of all) {
			counts.set(c.attribute, (counts.get(c.attribute) ?? 0) + 1);
		}

		const filtered = all
			.filter((c) => {
				const matchesName = c.name
					.toLowerCase()
					.includes(nameFilter.toLowerCase());
				const matchesAttribute =
					attributeFilter === null || c.attribute === attributeFilter;

				return matchesName && matchesAttribute;
			})
			.sort((a, b) => a.name.localeCompare(b.name));

		return { characters: filtered, attributeCounts: counts };
	}, [all, nameFilter, attributeFilter]);

	function handleSelect(id: number | string) {
		const newId = selectedId === id ? null : id;
		setInternalSelectedId(newId);
		onSelect?.(id);
	}

	return (
		<div className="space-y-6">
			<div className="space-y-3">
				<Input
					placeholder="Search by name..."
					value={nameFilter}
					onChange={(e) => setNameFilter(e.target.value)}
				/>

				<ToggleGroup
					type="single"
					variant="outline"
					className="w-full"
					onValueChange={(value) => {
						setAttributeFilter(
							value === "" ? null : (Number(value) as Attribute),
						);
					}}
				>
					{ATTRIBUTE_ENTRIES.map(([key, label]) => (
						<ToggleGroupItem
							key={key}
							value={String(key)}
							aria-label={label}
							className="flex-1 h-12 cursor-pointer"
						>
							<span
								className="size-7 rounded-full -mx-0.5"
								style={{ backgroundColor: ATTRIBUTE_COLOR[key] }}
							/>
							<span className="ml-2">{label}</span>
							<span className="text-muted-foreground">
								{attributeCounts.get(key) ?? 0}
							</span>
						</ToggleGroupItem>
					))}
				</ToggleGroup>
			</div>

			{characters.length === 0 ? (
				<p className="text-sm text-muted-foreground py-4 text-center">
					No characters found.
				</p>
			) : (
				<div className="grid grid-cols-3 sm:grid-cols-4 md:grid-cols-5 gap-2">
					{characters.map((c) => {
						const isSelected = selectedId === c.id;

						return (
							<Card
								key={c.id}
								size="sm"
								onClick={() => handleSelect(c.id)}
								className={`cursor-pointer transition-all duration-150 hover:scale-[1.03] hover:shadow-md ${isSelected ? "ring-2 ring-primary" : ""}`}
							>
								<CardContent className="flex flex-col items-center gap-1 py-1">
									<span className="size-20 rounded-full bg-muted-foreground/20" />
									<span className="text-sm font-medium mt-1">{c.name}</span>
									<span
										className="text-xs"
										style={{ color: ATTRIBUTE_COLOR[c.attribute] }}
									>
										{getAttributeName(c.attribute)}
									</span>
								</CardContent>
							</Card>
						);
					})}
				</div>
			)}
		</div>
	);
}
