import { Card } from "@shiron/ui/components/ui/card";
import { Badge } from "@shiron/ui/components/ui/badge";
import { ChatCircle, WarningCircle, CheckCircle } from "@phosphor-icons/react";

export interface IssueData {
	id: number;
	title: string;
	labels: { name: string; color: string }[];
	author: string;
	createdAt: string;
	commentCount: number;
	status: "open" | "closed";
}

const mockIssues: IssueData[] = [
	{
		id: 128,
		title: "API gateway crashes on malformed request headers",
		labels: [
			{ name: "bug", color: "bg-red-500/20 text-red-500" },
			{ name: "critical", color: "bg-orange-500/20 text-orange-500" },
		],
		author: "kuro",
		createdAt: "1h ago",
		commentCount: 3,
		status: "open",
	},
	{
		id: 127,
		title: "Add support for webhooks on push events",
		labels: [{ name: "feature", color: "bg-blue-500/20 text-blue-500" }],
		author: "midori",
		createdAt: "3h ago",
		commentCount: 7,
		status: "open",
	},
	{
		id: 126,
		title: "Improve CI pipeline caching strategy",
		labels: [{ name: "improvement", color: "bg-green-500/20 text-green-500" }],
		author: "aoi",
		createdAt: "5h ago",
		commentCount: 2,
		status: "open",
	},
	{
		id: 125,
		title: "Documentation for the REST API is outdated",
		labels: [{ name: "docs", color: "bg-purple-500/20 text-purple-500" }],
		author: "kuro",
		createdAt: "8h ago",
		commentCount: 1,
		status: "closed",
	},
	{
		id: 124,
		title: "File upload fails for files larger than 100MB",
		labels: [{ name: "bug", color: "bg-red-500/20 text-red-500" }],
		author: "shiron",
		createdAt: "1d ago",
		commentCount: 12,
		status: "closed",
	},
	{
		id: 123,
		title: "Dark mode toggle doesn't persist across sessions",
		labels: [
			{ name: "bug", color: "bg-red-500/20 text-red-500" },
			{ name: "ui", color: "bg-cyan-500/20 text-cyan-500" },
		],
		author: "midori",
		createdAt: "2d ago",
		commentCount: 5,
		status: "closed",
	},
];

export function IssueList() {
	return (
		<div className="space-y-1">
			<div className="flex items-center justify-between mb-4">
				<div className="flex items-center gap-3">
					<span className="flex items-center gap-1.5 text-xs font-medium">
						<WarningCircle size={14} className="text-green-500" />
						{mockIssues.filter((i) => i.status === "open").length} Open
					</span>
					<span className="flex items-center gap-1.5 text-xs text-muted-foreground">
						<CheckCircle size={14} />
						{mockIssues.filter((i) => i.status === "closed").length} Closed
					</span>
				</div>
			</div>

			{mockIssues.map((issue) => (
				<IssueCard key={`issue-${issue.id}`} issue={issue} />
			))}
		</div>
	);
}

function IssueCard({ issue }: { issue: IssueData }) {
	const StatusIcon = issue.status === "open" ? WarningCircle : CheckCircle;
	const statusColor =
		issue.status === "open" ? "text-green-500" : "text-purple-500";

	return (
		<Card className="group flex items-start gap-3 rounded-lg border border-border/40 bg-background/50 px-4 py-3 transition-colors hover:bg-muted/20 hover:border-border/60">
			<StatusIcon size={16} className={`mt-0.5 shrink-0 ${statusColor}`} />
			<div className="flex-1 min-w-0">
				<div className="flex items-start gap-2">
					<span className="text-sm font-medium leading-snug text-foreground group-hover:text-primary transition-colors">
						{issue.title}
					</span>
				</div>
				<div className="flex items-center gap-2 mt-1 flex-wrap">
					{issue.labels.map((label) => (
						<Badge
							key={`issue-${issue.id}-label-${label.name}`}
							variant="outline"
							className={`text-[10px] px-1.5 py-0 h-4 border-0 ${label.color}`}
						>
							{label.name}
						</Badge>
					))}
					<span className="text-[11px] text-muted-foreground">
						#{issue.id} opened {issue.createdAt} by{" "}
						<span className="font-medium">{issue.author}</span>
					</span>
				</div>
			</div>
			{issue.commentCount > 0 && (
				<span className="flex items-center gap-1 text-xs text-muted-foreground shrink-0 mt-0.5">
					<ChatCircle size={12} />
					{issue.commentCount}
				</span>
			)}
		</Card>
	);
}
