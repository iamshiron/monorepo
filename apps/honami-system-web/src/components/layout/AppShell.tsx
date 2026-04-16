import { Link, useRouterState } from "@tanstack/react-router";
import { ModeToggle } from "@/components/layout/ModeToggle";
import { Button } from "@shiron/ui/components/ui/button";

function GlobalHeader() {
	return (
		<header className="sticky top-4 z-50 mx-auto max-w-5xl rounded-2xl left-0 right-0 px-4 bg-background/95 backdrop-blur-md shadow-lg border border-border">
			<div className="flex h-14 items-center justify-between px-6">
				<Link to="/" className="flex items-center gap-2">
					<span className="text-xl font-bold text-primary">HonamiSystem</span>
				</Link>
				<nav className="flex items-center gap-1">
					<Link to="/chat">
						<Button variant="ghost">Chat</Button>
					</Link>
					<Link to="/test">
						<Button variant="ghost">Test</Button>
					</Link>
				</nav>
				<div className="flex items-center gap-2">
					<Link to="/login">
						<Button variant="ghost" size="sm">
							Sign in
						</Button>
					</Link>
					<Link to="/register">
						<Button variant="default" size="sm">
							Register
						</Button>
					</Link>
					<ModeToggle />
				</div>
			</div>
		</header>
	);
}

export function AppShell({ children }: { children: React.ReactNode }) {
	const pathname = useRouterState({
		select: (s) => s.location.pathname,
	});
	const isChatRoute = pathname.startsWith("/chat");

	if (isChatRoute) {
		return <div className="h-svh bg-background">{children}</div>;
	}

	return (
		<div className="min-h-screen bg-background">
			<GlobalHeader />
			<main className="container mx-auto pt-6">{children}</main>
		</div>
	);
}
