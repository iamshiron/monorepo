import { Link, useRouterState } from "@tanstack/react-router";
import {
	Code,
	WarningCircle,
	GitMerge,
	Lightning,
	BookOpen,
	Gear,
} from "@phosphor-icons/react";
import { Tabs, TabsList, TabsTrigger } from "@shiron/ui/components/ui/tabs";

interface RepoTabDef {
	label: string;
	icon: React.ElementType;
	to: string;
	hasParams: boolean;
}

const tabs: RepoTabDef[] = [
	{ label: "Code", icon: Code, to: "/$user/$repo", hasParams: false },
	{
		label: "Issues",
		icon: WarningCircle,
		to: "/$user/$repo/issues",
		hasParams: true,
	},
	{
		label: "Merge Requests",
		icon: GitMerge,
		to: "/$user/$repo/merge-requests",
		hasParams: true,
	},
	{
		label: "CI/CD",
		icon: Lightning,
		to: "/$user/$repo/pipelines",
		hasParams: true,
	},
	{ label: "Wiki", icon: BookOpen, to: "/$user/$repo/wiki", hasParams: true },
	{
		label: "Settings",
		icon: Gear,
		to: "/$user/$repo/settings",
		hasParams: true,
	},
];

export function RepoTabs({ owner, repo }: { owner: string; repo: string }) {
	const routerState = useRouterState();
	const currentPath = routerState.location.pathname;
	const basePath = `/${owner}/${repo}`;

	const activeTab = tabs.find((tab, i) => {
		if (i === 0) return currentPath === basePath;
		const routePath = `/${owner}/${repo}${tab.to.replace("/$user/$repo", "")}`;
		return currentPath.startsWith(routePath);
	});

	return (
		<Tabs value={activeTab?.to ?? ""}>
			<TabsList className="w-full justify-start rounded-none border-b border-border/50 bg-transparent p-0 h-auto">
				{tabs.map((tab) => {
					const Icon = tab.icon;
					return (
						<TabsTrigger
							key={tab.to}
							value={tab.to}
							className="relative gap-1.5 rounded-none border-b-2 border-transparent px-4 py-2.5 text-xs text-muted-foreground shadow-none transition-none data-[state=active]:border-primary data-[state=active]:text-foreground data-[state=active]:shadow-none"
							asChild
						>
							<Link to={tab.to} params={{ user: owner, repo }}>
								<Icon size={14} />
								{tab.label}
							</Link>
						</TabsTrigger>
					);
				})}
			</TabsList>
		</Tabs>
	);
}
