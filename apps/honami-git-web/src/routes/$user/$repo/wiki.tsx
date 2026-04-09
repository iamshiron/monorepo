import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/$user/$repo/wiki")({
	component: WikiPage,
});

function WikiPage() {
	return (
		<div className="flex flex-col items-center justify-center py-20 text-center">
			<h3 className="text-sm font-semibold mb-1">Wiki</h3>
			<p className="text-xs text-muted-foreground">
				Project documentation and wiki pages.
			</p>
		</div>
	);
}
