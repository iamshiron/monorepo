import { createFileRoute, Outlet } from "@tanstack/react-router";
import { Avatar, AvatarFallback } from "@shiron/ui/components/ui/avatar";
import { Tabs, TabsList, TabsTrigger } from "@shiron/ui/components/ui/tabs";
import { Badge } from "@shiron/ui/components/ui/badge";
import { Card } from "@shiron/ui/components/ui/card";
import { Separator } from "@shiron/ui/components/ui/separator";
import { Link, useMatchRoute, useRouterState } from "@tanstack/react-router";
import {
	MapPin,
	Link as LinkIcon,
	Calendar,
	Envelope,
} from "@phosphor-icons/react";

const languages = [
	{ name: "Rust", pct: 42 },
	{ name: "TypeScript", pct: 28 },
	{ name: "Go", pct: 15 },
	{ name: "Python", pct: 10 },
	{ name: "Shell", pct: 5 },
];

const langColorMap: Record<string, string> = {
	Rust: "bg-orange-500",
	TypeScript: "bg-blue-500",
	Go: "bg-cyan-400",
	Python: "bg-green-500",
	Shell: "bg-emerald-400",
};

export const Route = createFileRoute("/$user")({
	component: UserProfileLayout,
});

function UserProfileLayout() {
	const { user } = Route.useParams();
	const matchRoute = useMatchRoute();
	const routerState = useRouterState();
	const currentPath = routerState.location.pathname;

	const isRepoRoute =
		currentPath.startsWith(`/${user}/`) &&
		currentPath.split("/").length > 2 &&
		!currentPath.includes("/repositories") &&
		!currentPath.includes("/profile") &&
		!currentPath.includes("/activity");

	if (isRepoRoute) {
		return <Outlet />;
	}

	const tabs = [
		{ label: "Overview", path: "" },
		{ label: "Repositories", path: "/repositories" },
		{ label: "Activity", path: "/activity" },
	];

	const basePath = `/${user}`;
	const activeTab = tabs.find((tab) => {
		if (tab.path === "") return !!matchRoute({ to: basePath });
		return !!matchRoute({ to: `${basePath}${tab.path}` });
	});

	return (
		<div className="p-6">
			<div className="grid grid-cols-1 lg:grid-cols-[300px_1fr] gap-6">
				<aside className="space-y-4">
					<Card className="glass border-border/50 p-5 space-y-3">
						<div className="flex items-center gap-3">
							<Avatar className="size-14 border-2 border-primary/20">
								<AvatarFallback className="text-lg bg-primary/10 text-primary font-semibold">
									{user[0].toUpperCase()}
								</AvatarFallback>
							</Avatar>
							<div>
								<h2 className="text-base font-bold">{user}</h2>
								<p className="text-xs text-muted-foreground">@{user}</p>
							</div>
						</div>
						<div className="flex items-center gap-2">
							<Badge variant="secondary" className="text-[10px]">
								12 repos
							</Badge>
							<Badge variant="secondary" className="text-[10px]">
								48 followers
							</Badge>
						</div>
						<Separator />
						<h3 className="text-sm font-semibold">About</h3>
						<p className="text-xs text-muted-foreground leading-relaxed">
							Full-stack developer passionate about open source, systems
							programming, and building developer tools. Currently working on
							HonamiGit.
						</p>
						<div className="flex flex-wrap gap-1.5">
							<Badge variant="secondary" className="text-[10px]">
								Rust
							</Badge>
							<Badge variant="secondary" className="text-[10px]">
								TypeScript
							</Badge>
							<Badge variant="secondary" className="text-[10px]">
								Go
							</Badge>
							<Badge variant="secondary" className="text-[10px]">
								React
							</Badge>
							<Badge variant="secondary" className="text-[10px]">
								Systems Design
							</Badge>
						</div>
						<Separator />
						<div className="space-y-2 text-xs text-muted-foreground">
							<div className="flex items-center gap-2">
								<MapPin size={13} />
								Tokyo, Japan
							</div>
							<div className="flex items-center gap-2">
								<LinkIcon size={13} />
								<span className="text-primary">github.com/shiron</span>
							</div>
							<div className="flex items-center gap-2">
								<Envelope size={13} />
								<span className="text-primary">shiron@example.dev</span>
							</div>
							<div className="flex items-center gap-2">
								<Calendar size={13} />
								Joined March 2024
							</div>
						</div>
						<Separator />
						<div>
							<h4 className="text-xs font-semibold mb-2">Organizations</h4>
							<div className="flex gap-2">
								<Avatar className="size-8">
									<AvatarFallback className="text-[10px] bg-blue-500/10 text-blue-500">
										O
									</AvatarFallback>
								</Avatar>
								<Avatar className="size-8">
									<AvatarFallback className="text-[10px] bg-green-500/10 text-green-500">
										OS
									</AvatarFallback>
								</Avatar>
							</div>
						</div>
					</Card>

					<Card className="glass border-border/50 p-5 space-y-3">
						<h3 className="text-sm font-semibold">Languages</h3>
						<div className="flex h-2 rounded-full overflow-hidden gap-px">
							{languages.map((lang) => (
								<div
									key={`lang-bar-${lang.name}`}
									className={`${langColorMap[lang.name]} rounded-sm`}
									style={{ width: `${lang.pct}%` }}
								/>
							))}
						</div>
						<div className="space-y-1.5">
							{languages.map((lang) => (
								<div
									key={`lang-row-${lang.name}`}
									className="flex items-center justify-between text-xs"
								>
									<span className="flex items-center gap-1.5">
										<div
											className={`size-2 rounded-full ${langColorMap[lang.name]}`}
										/>
										{lang.name}
									</span>
									<span className="text-muted-foreground">{lang.pct}%</span>
								</div>
							))}
						</div>
					</Card>
				</aside>

				<div className="space-y-6">
					<Tabs value={activeTab?.path ?? ""}>
						<TabsList className="w-full justify-start rounded-none border-b border-border/50 bg-transparent p-0 h-auto">
							{tabs.map((tab) => (
								<TabsTrigger
									key={`user-tab-${tab.label}`}
									value={tab.path}
									className="relative gap-1.5 rounded-none border-b-2 border-transparent px-4 py-2.5 text-xs text-muted-foreground shadow-none transition-none data-[state=active]:border-primary data-[state=active]:text-foreground data-[state=active]:shadow-none"
									asChild
								>
									<Link to={`${basePath}${tab.path}`}>{tab.label}</Link>
								</TabsTrigger>
							))}
						</TabsList>
					</Tabs>

					<Outlet />
				</div>
			</div>
		</div>
	);
}
