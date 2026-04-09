import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/$user/$repo/pipelines")({
	component: PipelinesPage,
});

function PipelinesPage() {
	return (
		<div className="flex flex-col items-center justify-center py-20 text-center">
			<h3 className="text-sm font-semibold mb-1">CI/CD Pipelines</h3>
			<p className="text-xs text-muted-foreground">
				Pipeline configuration will appear here.
			</p>
		</div>
	);
}
