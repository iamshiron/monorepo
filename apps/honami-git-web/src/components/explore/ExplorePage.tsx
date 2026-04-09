import { useState } from "react";
import { RepoCard, type RepoCardData } from "@/components/shared/RepoCard";
import { Input } from "@shiron/ui/components/ui/input";
import { Button } from "@shiron/ui/components/ui/button";
import { Tabs, TabsList, TabsTrigger } from "@shiron/ui/components/ui/tabs";
import { MagnifyingGlass, Funnel } from "@phosphor-icons/react";

const trendingRepos: RepoCardData[] = [
	{
		name: "tokio",
		owner: "tokio-rs",
		description:
			"A runtime for writing reliable asynchronous applications with Rust",
		language: "rust",
		stars: 26800,
		forks: 2450,
		visibility: "public",
		updatedAt: "30m ago",
	},
	{
		name: "next.js",
		owner: "vercel",
		description:
			"The React framework for production - hybrid static & server rendering",
		language: "javascript",
		stars: 128000,
		forks: 26900,
		visibility: "public",
		updatedAt: "1h ago",
	},
	{
		name: "go",
		owner: "golang",
		description: "The Go programming language",
		language: "go",
		stars: 124000,
		forks: 17600,
		visibility: "public",
		updatedAt: "2h ago",
	},
	{
		name: "swift",
		owner: "apple",
		description: "The Swift Programming Language",
		language: "swift",
		stars: 68000,
		forks: 10300,
		visibility: "public",
		updatedAt: "3h ago",
	},
	{
		name: "pytorch",
		owner: "pytorch",
		description:
			"Tensors and Dynamic neural networks in Python with strong GPU acceleration",
		language: "python",
		stars: 86000,
		forks: 23100,
		visibility: "public",
		updatedAt: "4h ago",
	},
	{
		name: "kotlin",
		owner: "JetBrains",
		description: "The Kotlin Programming Language",
		language: "kotlin",
		stars: 49500,
		forks: 5800,
		visibility: "public",
		updatedAt: "5h ago",
	},
];

const languages = ["All", "Rust", "TypeScript", "Go", "Python", "JavaScript"];

export function ExplorePage() {
	const [selectedLang, setSelectedLang] = useState("All");

	return (
		<div className="space-y-6">
			<div className="space-y-4">
				<div className="flex items-center justify-between">
					<div>
						<h2 className="text-xl font-bold">Explore</h2>
						<p className="text-sm text-muted-foreground mt-0.5">
							Discover trending repositories and projects
						</p>
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
				</div>

				<Tabs value={selectedLang} onValueChange={setSelectedLang}>
					<TabsList className="h-8">
						{languages.map((lang) => (
							<TabsTrigger
								key={`lang-${lang}`}
								value={lang}
								className="text-xs h-7 px-3"
							>
								{lang}
							</TabsTrigger>
						))}
					</TabsList>
				</Tabs>
			</div>

			<div className="grid grid-cols-1 lg:grid-cols-2 gap-3">
				{trendingRepos.map((repo) => (
					<RepoCard key={`explore-${repo.owner}-${repo.name}`} repo={repo} />
				))}
			</div>
		</div>
	);
}
