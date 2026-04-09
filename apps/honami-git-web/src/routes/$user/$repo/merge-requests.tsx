import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/$user/$repo/merge-requests")({
	component: MergeRequestsPage,
});

function MergeRequestsPage() {
	return (
		<div className="flex flex-col items-center justify-center py-20 text-center">
			<h3 className="text-sm font-semibold mb-1">Merge Requests</h3>
			<p className="text-xs text-muted-foreground">
				No open merge requests. Create one to start collaborating.
			</p>
		</div>
	);
}
