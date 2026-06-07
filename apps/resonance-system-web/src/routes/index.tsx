import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/")({
	component: HomePage,
});

function HomePage() {
	return (
		<div className="flex flex-col items-center justify-center min-h-[60vh]">
			<div className="text-center mb-8">
				<h1 className="text-4xl font-bold mb-4">
					<span className="text-primary">Resonance System</span>
				</h1>
				<p className="text-muted-foreground text-lg max-w-md">
					Track your Wuthering Waves Characters stats.
				</p>
			</div>
		</div>
	);
}
