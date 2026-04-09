import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/$user/$repo/settings")({
	component: SettingsPage,
});

function SettingsPage() {
	return (
		<div className="flex flex-col items-center justify-center py-20 text-center">
			<h3 className="text-sm font-semibold mb-1">Settings</h3>
			<p className="text-xs text-muted-foreground">
				Repository settings and configuration.
			</p>
		</div>
	);
}
