import { createFileRoute } from "@tanstack/react-router";
import { RepoCard, type RepoCardData } from "@/components/shared/RepoCard";
import { Input } from "@shiron/ui/components/ui/input";
import { Button } from "@shiron/ui/components/ui/button";
import { Badge } from "@shiron/ui/components/ui/badge";
import { Tabs, TabsList, TabsTrigger } from "@shiron/ui/components/ui/tabs";
import { MagnifyingGlass, Funnel } from "@phosphor-icons/react";
import { useState } from "react";

const allRepos: RepoCardData[] = [
	{
		name: "honami-git",
		owner: "shiron",
		description:
			"A modern Git hosting platform with beautiful UI and powerful CI/CD integration",
		language: "rust",
		stars: 2840,
		forks: 180,
		visibility: "public",
		updatedAt: "2h ago",
	},
	{
		name: "dotfiles",
		owner: "shiron",
		description: "Personal configuration files for development environment",
		language: "shell",
		stars: 42,
		forks: 8,
		visibility: "public",
		updatedAt: "3d ago",
	},
	{
		name: "frontend-kit",
		owner: "org",
		description:
			"Design system and component library built with React and Tailwind CSS",
		language: "typescript",
		stars: 520,
		forks: 64,
		visibility: "public",
		updatedAt: "5h ago",
	},
	{
		name: "api-gateway",
		owner: "org",
		description:
			"High-performance API gateway with rate limiting and authentication",
		language: "go",
		stars: 890,
		forks: 120,
		visibility: "private",
		updatedAt: "1d ago",
	},
	{
		name: "monorepo",
		owner: "shiron",
		description:
			"Mono-repository containing all personal projects and shared libraries",
		language: "typescript",
		stars: 156,
		forks: 22,
		visibility: "private",
		updatedAt: "1d ago",
	},
	{
		name: "scripts",
		owner: "shiron",
		description: "Collection of useful automation scripts and CLI tools",
		language: "python",
		stars: 18,
		forks: 3,
		visibility: "private",
		updatedAt: "1w ago",
	},
	{
		name: "infra-config",
		owner: "org",
		description: "Infrastructure configuration and Terraform modules",
		language: "go",
		stars: 34,
		forks: 7,
		visibility: "private",
		updatedAt: "4d ago",
	},
	{
		name: "docs",
		owner: "org",
		description: "Public documentation site for all org projects",
		language: "typescript",
		stars: 88,
		forks: 15,
		visibility: "public",
		updatedAt: "6h ago",
	},
];

const ownershipFilters = ["All", "Personal", "Orgs"];

export const Route = createFileRoute("/repositories")({
	component: RepositoriesPage,
});

function RepositoriesPage() {
	const [filter, setFilter] = useState("All");

	const filtered =
		filter === "All"
			? allRepos
			: filter === "Personal"
				? allRepos.filter((r) => r.owner === "shiron")
				: allRepos.filter((r) => r.owner === "org");

	return (
		<div className="p-6 space-y-6">
			<div className="space-y-4">
				<div className="flex items-center justify-between">
					<div>
						<h2 className="text-xl font-bold">Repositories</h2>
						<p className="text-sm text-muted-foreground mt-0.5">
							All repositories you have access to
						</p>
					</div>
					<div className="flex items-center gap-2">
						<Badge variant="secondary" className="text-[10px]">
							{filtered.length} repositories
						</Badge>
					</div>
				</div>

				<div className="flex items-center gap-3">
					<div className="relative flex-1 max-w-sm">
						<MagnifyingGlass
							size={14}
							className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground"
						/>
						<Input
							placeholder="Search repositories..."
							className="pl-9 h-8 text-sm"
						/>
					</div>
					<Button variant="outline" size="sm" className="gap-1.5 h-8">
						<Funnel size={13} />
						Filters
					</Button>
					<Tabs value={filter} onValueChange={setFilter}>
						<TabsList className="h-8">
							{ownershipFilters.map((f) => (
								<TabsTrigger
									key={`own-${f}`}
									value={f}
									className="text-xs h-7 px-3"
								>
									{f}
								</TabsTrigger>
							))}
						</TabsList>
					</Tabs>
				</div>
			</div>

			<div className="grid grid-cols-1 lg:grid-cols-2 gap-3">
				{filtered.map((repo) => (
					<RepoCard key={`all-${repo.owner}-${repo.name}`} repo={repo} />
				))}
			</div>
		</div>
	);
}
