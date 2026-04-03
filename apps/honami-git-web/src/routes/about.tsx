import { createFileRoute } from "@tanstack/react-router";
import {
	Card,
	CardContent,
	CardHeader,
	CardTitle,
} from "@shiron/ui/components/ui/card";

export const Route = createFileRoute("/about")({
	component: AboutPage,
});

function AboutPage() {
	return (
		<div>
			<h1 className="text-2xl font-bold mb-6">About</h1>

			<Card className="glass mb-6">
				<CardHeader>
					<CardTitle>Honami Git</CardTitle>
				</CardHeader>
				<CardContent>
					<p className="text-muted-foreground">
						A modern Git platform built with TanStack Router, React Query, and
						shadcn/ui. This project serves as a foundation for building
						Git-powered workflows and repository management tools.
					</p>
				</CardContent>
			</Card>

			<div className="grid grid-cols-1 md:grid-cols-2 gap-4">
				<Card className="glass">
					<CardContent className="pt-6">
						<h3 className="font-semibold mb-2">Tech Stack</h3>
						<ul className="text-sm text-muted-foreground space-y-1">
							<li>TanStack Router + React Query</li>
							<li>shadcn/ui + Tailwind CSS v4</li>
							<li>TypeScript + Vite</li>
						</ul>
					</CardContent>
				</Card>
				<Card className="glass">
					<CardContent className="pt-6">
						<h3 className="font-semibold mb-2">Features</h3>
						<ul className="text-sm text-muted-foreground space-y-1">
							<li>Dark mode with system preference</li>
							<li>Type-safe routing</li>
							<li>Responsive layout with AppShell</li>
						</ul>
					</CardContent>
				</Card>
			</div>
		</div>
	);
}
