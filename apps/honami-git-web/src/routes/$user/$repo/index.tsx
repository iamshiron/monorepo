import { useState } from "react";
import { createFileRoute } from "@tanstack/react-router";
import { CodeBrowser } from "@/components/repository/CodeBrowser";
import { ReadmePreview } from "@/components/repository/ReadmePreview";
import { EmptyRepo } from "@/components/repository/EmptyRepo";
import { CloneDialog } from "@/components/repository/CloneDialog";
import { Badge } from "@shiron/ui/components/ui/badge";
import { Button } from "@shiron/ui/components/ui/button";
import { GitBranch, Clock, Lightning, Download } from "@phosphor-icons/react";

export const Route = createFileRoute("/$user/$repo/")({
	component: RepoCodePage,
});

function RepoCodePage() {
	const { user, repo } = Route.useParams();
	const isEmpty = repo === "empty";
	const [cloneOpen, setCloneOpen] = useState(false);

	if (isEmpty) {
		return (
			<div>
				<EmptyRepo owner={user} repo={repo} />
				<CloneDialog
					open={cloneOpen}
					onOpenChange={setCloneOpen}
					owner={user}
					repo={repo}
				/>
			</div>
		);
	}

	return (
		<div className="space-y-4">
			<div className="flex items-center justify-between">
				<div className="flex items-center gap-3 text-xs text-muted-foreground">
					<span className="flex items-center gap-1.5">
						<GitBranch size={13} />
						<span className="font-mono font-medium text-foreground">main</span>
					</span>
					<span className="flex items-center gap-1">
						<Clock size={12} />
						Updated 2h ago
					</span>
					<Badge variant="secondary" className="gap-0.5 text-[10px] h-5">
						<Lightning size={10} className="text-success" />
						Passing
					</Badge>
				</div>
				<div className="flex items-center gap-2">
					<Button variant="outline" size="sm" className="text-xs h-7">
						Go to file
					</Button>
					<Button
						variant="outline"
						size="sm"
						className="gap-1.5 text-xs h-7"
						onClick={() => setCloneOpen(true)}
					>
						<Download size={13} />
						Clone
					</Button>
				</div>
			</div>
			<CodeBrowser />
			<ReadmePreview />
			<CloneDialog
				open={cloneOpen}
				onOpenChange={setCloneOpen}
				owner={user}
				repo={repo}
			/>
		</div>
	);
}
