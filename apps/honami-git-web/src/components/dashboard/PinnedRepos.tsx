import { RepoCard, type RepoCardData } from "@/components/shared/RepoCard";

const pinnedRepos: RepoCardData[] = [
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
];

export function PinnedRepos() {
	return (
		<div>
			<h3 className="text-sm font-semibold mb-4 flex items-center gap-2">
				Pinned Repositories
			</h3>
			<div className="grid grid-cols-1 lg:grid-cols-2 gap-3">
				{pinnedRepos.map((repo) => (
					<RepoCard key={`${repo.owner}/${repo.name}`} repo={repo} />
				))}
			</div>
		</div>
	);
}
