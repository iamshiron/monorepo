import { createFileRoute } from "@tanstack/react-router";
import { Card } from "@shiron/ui/components/ui/card";
import {
	GitCommit,
	GitMerge,
	WarningCircle,
	ChatCircle,
} from "@phosphor-icons/react";

type ActivityType = "commit" | "merge" | "issue" | "comment";

const activityIcons: Record<
	ActivityType,
	{ icon: React.ElementType; color: string }
> = {
	commit: { icon: GitCommit, color: "text-green-500" },
	merge: { icon: GitMerge, color: "text-purple-500" },
	issue: { icon: WarningCircle, color: "text-blue-500" },
	comment: { icon: ChatCircle, color: "text-yellow-500" },
};

const activities = [
	{
		id: "a1",
		type: "commit" as const,
		text: "Pushed 3 commits to main in honami-git",
		time: "5m ago",
	},
	{
		id: "a2",
		type: "merge" as const,
		text: "Merged MR !42 in honami-git",
		time: "22m ago",
	},
	{
		id: "a3",
		type: "commit" as const,
		text: "Pushed 1 commit to feature/auth in monorepo",
		time: "4h ago",
	},
	{
		id: "a4",
		type: "issue" as const,
		text: "Closed issue #125 in honami-git",
		time: "5h ago",
	},
	{
		id: "a5",
		type: "merge" as const,
		text: "Merged MR !40 in frontend-kit",
		time: "6h ago",
	},
	{
		id: "a6",
		type: "comment" as const,
		text: "Commented on MR !38 in honami-git",
		time: "1d ago",
	},
	{
		id: "a7",
		type: "commit" as const,
		text: "Pushed 5 commits to main in dotfiles",
		time: "3d ago",
	},
	{
		id: "a8",
		type: "issue" as const,
		text: "Opened issue #14 in scripts",
		time: "5d ago",
	},
];

export const Route = createFileRoute("/$user/activity")({
	component: UserActivityPage,
});

function UserActivityPage() {
	return (
		<div className="space-y-4">
			<h3 className="text-sm font-semibold">Activity</h3>
			<Card className="glass border-border/50 divide-y divide-border/30">
				{activities.map((activity) => {
					const config = activityIcons[activity.type];
					const Icon = config.icon;
					return (
						<div
							key={activity.id}
							className="flex items-center gap-3 px-4 py-3 first:rounded-t-lg last:rounded-b-lg hover:bg-muted/20 transition-colors"
						>
							<Icon size={14} className={config.color} />
							<span className="text-xs text-foreground/80 flex-1">
								{activity.text}
							</span>
							<span className="text-[11px] text-muted-foreground/60 shrink-0">
								{activity.time}
							</span>
						</div>
					);
				})}
			</Card>
		</div>
	);
}
