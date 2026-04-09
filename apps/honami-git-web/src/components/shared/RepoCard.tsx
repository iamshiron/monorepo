import { Star, GitFork, Lock } from "@phosphor-icons/react";
import { Link } from "@tanstack/react-router";
import { Card } from "@shiron/ui/components/ui/card";
import { Badge } from "@shiron/ui/components/ui/badge";
import { LanguageDot } from "@/components/shared/LanguageDot";

export interface RepoCardData {
	name: string;
	owner: string;
	description: string;
	language: string;
	stars: number;
	forks: number;
	visibility: "public" | "private";
	updatedAt: string;
}

export function RepoCard({ repo }: { repo: RepoCardData }) {
	return (
		<Link to="/$user/$repo" params={{ user: repo.owner, repo: repo.name }}>
			<Card className="glass group relative overflow-hidden border-border/50 p-5 transition-all duration-300 hover:-translate-y-0.5 hover:shadow-lg hover:shadow-primary/5 hover:border-primary/20">
				<div className="flex items-start justify-between gap-3 mb-2">
					<div className="flex items-center gap-2 min-w-0">
						<span className="text-sm font-semibold text-primary truncate">
							{repo.owner}/{repo.name}
						</span>
						{repo.visibility === "private" && (
							<Badge
								variant="outline"
								className="shrink-0 text-[10px] px-1.5 py-0"
							>
								<Lock size={10} className="mr-0.5" />
								Private
							</Badge>
						)}
					</div>
				</div>
				<p className="text-muted-foreground text-xs leading-relaxed mb-4 line-clamp-2">
					{repo.description}
				</p>
				<div className="flex items-center gap-4 text-xs text-muted-foreground">
					<span className="flex items-center gap-1.5">
						<LanguageDot language={repo.language} />
						{repo.language}
					</span>
					<span className="flex items-center gap-1">
						<Star size={12} />
						{repo.stars}
					</span>
					<span className="flex items-center gap-1">
						<GitFork size={12} />
						{repo.forks}
					</span>
					<span className="ml-auto text-[11px]">{repo.updatedAt}</span>
				</div>
				<div className="absolute inset-x-0 bottom-0 h-px bg-gradient-to-r from-transparent via-primary/30 to-transparent opacity-0 transition-opacity duration-300 group-hover:opacity-100" />
			</Card>
		</Link>
	);
}
