import { createFileRoute } from "@tanstack/react-router";
import { Separator } from "@shiron/ui/components/ui/separator";
import { RepoCard, type RepoCardData } from "@/components/shared/RepoCard";
import { ContributionMap } from "@/components/dashboard/ContributionMap";

const profileRepos: RepoCardData[] = [
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
];

export const Route = createFileRoute("/$user/")({
	component: UserOverviewPage,
});

function UserOverviewPage() {
	return (
		<div className="space-y-6">
			<div>
				<h3 className="text-sm font-semibold mb-3">Pinned Repositories</h3>
				<div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
					{profileRepos.map((repo) => (
						<RepoCard key={`profile-${repo.owner}-${repo.name}`} repo={repo} />
					))}
				</div>
			</div>

			<Separator />

			<ContributionMap />
		</div>
	);
}
