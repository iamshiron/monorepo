import { useMemo, useState } from "react";
import { createFileRoute } from "@tanstack/react-router";
import { queryOptions, useQuery } from "@tanstack/react-query";
import { getGetApiContentResonatorsQueryOptions } from "@/api/content/content";
import type { Attribute } from "@/api/model/attribute";
import { ATTRIBUTE_ENTRIES, getAttributeName } from "@/types/attribute";
import { queryClient } from "@/lib/query-client";
import { Input } from "@shiron/ui/components/ui/input";
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

	const characters = useMemo(() => {
		const all = data?.data ?? [];

		return all.filter((c) => {
			const matchesName = c.name
				.toLowerCase()
				.includes(nameFilter.toLowerCase());

			const matchesAttribute =
				attributeFilter === null ||
				("attribute" in c &&
					(c as Record<string, unknown>).attribute === attributeFilter);

			return matchesName && matchesAttribute;
		});
	}, [data?.data, nameFilter, attributeFilter]);

	if (isLoading) {
		return (
			<div className="flex items-center justify-center min-h-[60vh]">
				<Spinner className="size-6" />
			</div>
		);
	}

	return (
		<div className="max-w-2xl mx-auto py-8 space-y-6">
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
					onValueChange={(value) => {
						setAttributeFilter(
							value === "" ? null : (Number(value) as Attribute),
						);
					}}
				>
					{ATTRIBUTE_ENTRIES.map(([key, label]) => (
						<ToggleGroupItem key={key} value={String(key)} aria-label={label}>
							<span className="size-3.5 rounded-full bg-muted-foreground/30" />
							{label}
						</ToggleGroupItem>
					))}
				</ToggleGroup>
			</div>

			<ul className="space-y-2">
				{characters.map((c) => (
					<li
						key={c.id}
						className="flex items-center gap-3 px-3 py-2 rounded-md bg-muted"
					>
						<span className="size-6 rounded-full bg-muted-foreground/20 shrink-0" />
						<span>{c.name}</span>
						{"attribute" in c && (
							<span className="ml-auto text-xs text-muted-foreground">
								{getAttributeName(
									(c as Record<string, unknown>).attribute as Attribute,
								)}
							</span>
						)}
					</li>
				))}
				{characters.length === 0 && (
					<li className="text-sm text-muted-foreground py-4 text-center">
						No characters found.
					</li>
				)}
			</ul>
		</div>
	);
}
