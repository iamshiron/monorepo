import { useMemo, useState } from "react";
import { createFileRoute } from "@tanstack/react-router";
import { queryOptions, useQuery } from "@tanstack/react-query";
import { getGetApiContentResonatorsQueryOptions } from "@/api/content/content";
import type { Attribute } from "@/api/model/attribute";
import {
	ATTRIBUTE_COLOR,
	ATTRIBUTE_ENTRIES,
	getAttributeName,
} from "@/types/attribute";
import { queryClient } from "@/lib/query-client";
import { Input } from "@shiron/ui/components/ui/input";
import { Card, CardContent } from "@shiron/ui/components/ui/card";
import { Spinner } from "@shiron/ui/components/ui/spinner";
import {
	ToggleGroup,
	ToggleGroupItem,
} from "@shiron/ui/components/ui/toggle-group";

const resonatorsQueryOptions = () =>
	queryOptions(getGetApiContentResonatorsQueryOptions());

export const Route = createFileRoute("/builder")({
	loader: () => {
		void queryClient.ensureQueryData(resonatorsQueryOptions());
	},
	component: BuilderPage,
});

function BuilderPage() {
	const { data, isLoading } = useQuery(resonatorsQueryOptions());
	const [nameFilter, setNameFilter] = useState("");
	const [attributeFilter, setAttributeFilter] = useState<Attribute | null>(
		null,
	);
	const [selectedId, setSelectedId] = useState<string | number | null>(null);

	const { characters, attributeCounts } = useMemo(() => {
		const all = data?.data ?? [];

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
	}, [data?.data, nameFilter, attributeFilter]);

	if (isLoading) {
		return (
			<div className="flex items-center justify-center min-h-[60vh]">
				<Spinner className="size-6" />
			</div>
		);
	}

	return (
		<div className="max-w-5xl mx-auto py-8 space-y-6">
			<h1 className="text-2xl font-bold">Characters</h1>

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
								onClick={() => setSelectedId(isSelected ? null : c.id)}
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
