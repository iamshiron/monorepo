import { Link } from "@tanstack/react-router";
import { Star, GitFork, Eye, Bell, DotsThree } from "@phosphor-icons/react";
import {
	Breadcrumb,
	BreadcrumbItem,
	BreadcrumbLink,
	BreadcrumbList,
	BreadcrumbPage,
	BreadcrumbSeparator,
} from "@shiron/ui/components/ui/breadcrumb";
import { Button } from "@shiron/ui/components/ui/button";
import { Badge } from "@shiron/ui/components/ui/badge";
import { Separator } from "@shiron/ui/components/ui/separator";

export function RepoHeader({
	owner,
	repo,
	visibility,
}: {
	owner: string;
	repo: string;
	visibility: "public" | "private";
}) {
	return (
		<div className="space-y-3">
			<Breadcrumb>
				<BreadcrumbList>
					<BreadcrumbItem>
						<BreadcrumbLink asChild>
							<Link to="/$user" params={{ user: owner }}>
								{owner}
							</Link>
						</BreadcrumbLink>
					</BreadcrumbItem>
					<BreadcrumbSeparator />
					<BreadcrumbItem>
						<BreadcrumbPage className="font-semibold">{repo}</BreadcrumbPage>
					</BreadcrumbItem>
				</BreadcrumbList>
			</Breadcrumb>

			<div className="flex items-center justify-between">
				<div className="flex items-center gap-3">
					<h1 className="text-lg font-bold">
						{owner}/{repo}
					</h1>
					<Badge variant="outline" className="text-[10px] capitalize">
						{visibility}
					</Badge>
				</div>
				<div className="flex items-center gap-1.5">
					<Button variant="outline" size="sm" className="gap-1.5 text-xs h-7">
						<Star size={13} />
						Star
						<span className="ml-1 text-muted-foreground">2.8k</span>
					</Button>
					<Button variant="outline" size="sm" className="gap-1.5 text-xs h-7">
						<GitFork size={13} />
						Fork
						<span className="ml-1 text-muted-foreground">180</span>
					</Button>
					<Separator orientation="vertical" className="h-5 mx-1" />
					<Button variant="ghost" size="icon" className="h-7 w-7">
						<Eye size={14} />
					</Button>
					<Button variant="ghost" size="icon" className="h-7 w-7">
						<Bell size={14} />
					</Button>
					<Button variant="ghost" size="icon" className="h-7 w-7">
						<DotsThree size={14} />
					</Button>
				</div>
			</div>
		</div>
	);
}
