import { Spinner } from "@shiron/ui/components/ui/spinner";
import { queryOptions, useQuery } from "@tanstack/react-query";
import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { getGetApiContentResonatorsQueryOptions } from "@/api/content/content";
import type { CharacterDTO } from "@/api/model";
import { CharacterBaseStatEditor } from "@/components/CharacterBaseStatEditor.tsx";
import { CharacterSelector } from "@/components/CharacterSelector";
import { queryClient } from "@/lib/query-client";

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

	const [selectedCharacter, setSelectedCharacter] =
		useState<CharacterDTO | null>(null);

	useEffect(() => {
		if (selectedId == null) {
			setSelectedCharacter(null);
		}

		setSelectedCharacter(data?.data.find((c) => c.id === selectedId) ?? null);
	}, [selectedId, data]);

	if (isLoading) {
		return (
			<div className="flex items-center justify-center min-h-[60vh]">
				<Spinner className="size-6" />
			</div>
		);
	}

	return (
		<div className="w-full mx-auto py-8 space-y-6">
			<h1 className="text-2xl font-bold">Characters</h1>

			<CharacterSelector
				characters={data?.data ?? []}
				selectedId={selectedId}
				onSelect={setSelectedId}
			/>

			{selectedCharacter != null && (
				<CharacterBaseStatEditor character={selectedCharacter} />
			)}
		</div>
	);
}
