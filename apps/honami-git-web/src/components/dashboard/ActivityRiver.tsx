import {
	GitCommit,
	GitMerge,
	WarningCircle,
	ChatCircle,
	Star,
} from "@phosphor-icons/react";
import { Avatar, AvatarFallback } from "@shiron/ui/components/ui/avatar";

type ActivityType = "commit" | "merge" | "issue" | "comment" | "star";

interface Activity {
	id: string;
	type: ActivityType;
	user: string;
	action: string;
	target: string;
	time: string;
}

const activityIcons: Record<
	ActivityType,
	{ icon: React.ElementType; color: string }
> = {
	commit: { icon: GitCommit, color: "text-green-500" },
	merge: { icon: GitMerge, color: "text-purple-500" },
	issue: { icon: WarningCircle, color: "text-blue-500" },
	comment: { icon: ChatCircle, color: "text-yellow-500" },
	star: { icon: Star, color: "text-orange-500" },
};

const mockActivities: Activity[] = [
	{
		id: "1",
		type: "commit",
		user: "shiron",
		action: "pushed 3 commits to",
		target: "main",
		time: "5m ago",
	},
	{
		id: "2",
		type: "merge",
		user: "shiron",
		action: "merged MR !42 into",
		target: "honami-git/core",
		time: "22m ago",
	},
	{
		id: "3",
		type: "issue",
		user: "kuro",
		action: "opened issue #128 in",
		target: "org/api-gateway",
		time: "1h ago",
	},
	{
		id: "4",
		type: "comment",
		user: "midori",
		action: "commented on MR !39 in",
		target: "org/frontend-kit",
		time: "2h ago",
	},
	{
		id: "5",
		type: "star",
		user: "aoi",
		action: "starred",
		target: "shiron/dotfiles",
		time: "3h ago",
	},
	{
		id: "6",
		type: "commit",
		user: "shiron",
		action: "pushed 1 commit to",
		target: "feature/auth",
		time: "4h ago",
	},
	{
		id: "7",
		type: "issue",
		user: "kuro",
		action: "closed issue #125 in",
		target: "honami-git/core",
		time: "5h ago",
	},
	{
		id: "8",
		type: "merge",
		user: "midori",
		action: "merged MR !40 into",
		target: "org/frontend-kit",
		time: "6h ago",
	},
];

export function ActivityRiver() {
	return (
		<div>
			<h3 className="text-sm font-semibold mb-4 flex items-center gap-2">
				<div className="size-1.5 rounded-full bg-primary animate-pulse" />
				Activity
			</h3>
			<div className="relative">
				<div className="absolute left-[13px] top-2 bottom-2 w-px bg-gradient-to-b from-primary/30 via-border to-transparent" />
				<div className="space-y-1">
					{mockActivities.map((activity, i) => {
						const config = activityIcons[activity.type];
						const Icon = config.icon;
						return (
							<div
								key={activity.id}
								className="group relative flex items-start gap-3 rounded-lg px-2 py-2.5 transition-colors hover:bg-muted/30"
								style={{ animationDelay: `${i * 50}ms` }}
							>
								<div className="relative z-10 mt-0.5">
									<Avatar className="size-7 border-2 border-background">
										<AvatarFallback className="text-[9px] bg-muted">
											{activity.user[0].toUpperCase()}
										</AvatarFallback>
									</Avatar>
								</div>
								<div className="flex-1 min-w-0 pt-0.5">
									<p className="text-xs leading-relaxed">
										<span className="font-medium text-foreground">
											{activity.user}
										</span>{" "}
										<span className="text-muted-foreground">
											{activity.action}
										</span>{" "}
										<span className="font-medium font-mono text-primary/80">
											{activity.target}
										</span>
									</p>
									<span className="text-[10px] text-muted-foreground/60">
										{activity.time}
									</span>
								</div>
								<Icon
									size={14}
									className={`mt-1 shrink-0 opacity-0 group-hover:opacity-100 transition-opacity ${config.color}`}
								/>
							</div>
						);
					})}
				</div>
			</div>
		</div>
	);
}
