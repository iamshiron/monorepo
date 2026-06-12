import { useState } from "react";
import { createFileRoute } from "@tanstack/react-router";
import { queryOptions, useQuery } from "@tanstack/react-query";
import { getGetApiContentResonatorsQueryOptions } from "@/api/content/content";
import { queryClient } from "@/lib/query-client";
import { Spinner } from "@shiron/ui/components/ui/spinner";
import { CharacterSelector } from "@/components/CharacterSelector";

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
	const [selectedId, setSelectedId] = useState<string | number | null>(null);

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

			<CharacterSelector
				characters={data?.data ?? []}
				selectedId={selectedId}
				onSelect={setSelectedId}
			/>
		</div>
	);
}
