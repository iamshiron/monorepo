import { Link, useMatchRoute } from "@tanstack/react-router";
import {
	MagnifyingGlass,
	Bell,
	Plus,
	SquaresFour,
	GitBranch,
} from "@phosphor-icons/react";
import { Button } from "@shiron/ui/components/ui/button";
import { Avatar, AvatarFallback } from "@shiron/ui/components/ui/avatar";
import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuItem,
	DropdownMenuSeparator,
	DropdownMenuTrigger,
} from "@shiron/ui/components/ui/dropdown-menu";
import { ModeToggle } from "@/components/layout/ModeToggle";

export function TopBar({
	onCommandPalette,
	onNewRepo,
}: {
	onCommandPalette: () => void;
	onNewRepo: () => void;
}) {
	const matchRoute = useMatchRoute();
	const isDashboard = !!matchRoute({ to: "/" });
	const isRepos = !!matchRoute({ to: "/repositories" });

	return (
		<header className="glass sticky top-0 z-50 border-b border-border/50">
			<div className="flex h-13 items-center justify-between px-4">
				<div className="flex items-center gap-6">
					<Link to="/" className="flex items-center gap-2.5 group">
						<div className="flex size-7 items-center justify-center rounded-lg bg-primary/10 transition-colors group-hover:bg-primary/20">
							<GitLogo className="size-4 text-primary" />
						</div>
						<span className="text-[15px] font-semibold tracking-tight text-foreground">
							Honami<span className="text-primary">Git</span>
						</span>
					</Link>

					<nav className="hidden md:flex items-center gap-0.5">
						<Link to="/">
							<Button
								variant="ghost"
								size="sm"
								className={`gap-1.5 text-muted-foreground ${isDashboard ? "text-foreground bg-muted/50" : ""}`}
							>
								<SquaresFour size={15} />
								<span className="hidden lg:inline">Dashboard</span>
							</Button>
						</Link>
						<Link to="/repositories">
							<Button
								variant="ghost"
								size="sm"
								className={`gap-1.5 text-muted-foreground ${isRepos ? "text-foreground bg-muted/50" : ""}`}
							>
								<GitBranch size={15} />
								<span className="hidden lg:inline">Repositories</span>
							</Button>
						</Link>
					</nav>
				</div>

				<div className="flex-1 max-w-md mx-6">
					<button
						type="button"
						onClick={onCommandPalette}
						className="flex h-8 w-full items-center gap-2 rounded-lg border border-border/60 bg-muted/30 px-3 text-sm text-muted-foreground transition-colors hover:bg-muted/50 hover:border-border"
					>
						<MagnifyingGlass size={14} />
						<span className="flex-1 text-left">Search...</span>
						<kbd className="pointer-events-none hidden select-none sm:inline-flex h-5 items-center gap-0.5 rounded border border-border/60 bg-background/80 px-1.5 font-mono text-[10px] font-medium text-muted-foreground">
							⌘K
						</kbd>
					</button>
				</div>

				<div className="flex items-center gap-1.5">
					<Button variant="ghost" size="icon" className="relative h-8 w-8">
						<Bell size={16} />
						<span className="absolute top-1.5 right-1.5 size-1.5 rounded-full bg-primary" />
					</Button>

					<Button
						variant="outline"
						size="sm"
						className="gap-1.5 h-8 text-xs"
						onClick={onNewRepo}
					>
						<Plus size={14} />
						<span className="hidden sm:inline">New Repo</span>
					</Button>

					<ModeToggle />

					<DropdownMenu>
						<DropdownMenuTrigger asChild>
							<Button
								variant="ghost"
								size="icon"
								className="h-8 w-8 rounded-full"
							>
								<Avatar className="size-6">
									<AvatarFallback className="text-[10px] bg-primary/10 text-primary">
										S
									</AvatarFallback>
								</Avatar>
							</Button>
						</DropdownMenuTrigger>
						<DropdownMenuContent align="end" className="w-48">
							<DropdownMenuItem asChild>
								<Link to="/$user" params={{ user: "shiron" }}>
									Profile
								</Link>
							</DropdownMenuItem>
							<DropdownMenuItem>Settings</DropdownMenuItem>
							<DropdownMenuSeparator />
							<DropdownMenuItem>Sign Out</DropdownMenuItem>
						</DropdownMenuContent>
					</DropdownMenu>
				</div>
			</div>
		</header>
	);
}

function GitLogo({ className }: { className?: string }) {
	return (
		<svg
			className={className}
			viewBox="0 0 16 16"
			fill="currentColor"
			aria-hidden="true"
		>
			<path d="M8 0a8 8 0 110 16A8 8 0 018 0zm.75 3.25a.75.75 0 00-1.5 0v5.5a.75.75 0 001.5 0v-5.5zM8 11a1 1 0 100 2 1 1 0 000-2z" />
		</svg>
	);
}
