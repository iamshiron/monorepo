import { createFileRoute, Link } from "@tanstack/react-router";
import { Card, CardContent } from "@shiron/ui/components/ui/card";
import { Button } from "@shiron/ui/components/ui/button";

export const Route = createFileRoute("/")({
	component: HomePage,
});

function HomePage() {
	return (
		<div>
			<div className="flex flex-col items-center justify-center min-h-[50vh] mb-8">
				<h1 className="text-4xl font-bold mb-4">
					<span className="text-primary">Honami Git</span>
				</h1>
				<p className="text-muted-foreground text-lg max-w-md text-center">
					A Git-powered platform for managing repositories, commits, and
					collaboration.
				</p>
			</div>

			<div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-8">
				<Card className="glass">
					<CardContent className="pt-6">
						<h3 className="text-muted-foreground text-sm mb-1">Repositories</h3>
						<p className="text-3xl font-bold text-primary">0</p>
					</CardContent>
				</Card>
				<Card className="glass">
					<CardContent className="pt-6">
						<h3 className="text-muted-foreground text-sm mb-1">Commits</h3>
						<p className="text-3xl font-bold text-primary">0</p>
					</CardContent>
				</Card>
				<Card className="glass">
					<CardContent className="pt-6">
						<h3 className="text-muted-foreground text-sm mb-1">Contributors</h3>
						<p className="text-3xl font-bold text-primary">0</p>
					</CardContent>
				</Card>
			</div>

			<Card className="glass">
				<CardContent className="pt-6">
					<h2 className="text-xl font-semibold mb-4">Quick Actions</h2>
					<div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
						<Button
							asChild
							variant="ghost"
							className="h-auto p-4 justify-start hover:bg-primary/10"
						>
							<Link to="/about" className="flex flex-col items-start">
								<span className="font-medium">Learn More</span>
								<p className="text-sm text-muted-foreground">
									Read about Honami Git
								</p>
							</Link>
						</Button>
					</div>
				</CardContent>
			</Card>
		</div>
	);
}
