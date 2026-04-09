import { Card } from "@shiron/ui/components/ui/card";
import { Avatar, AvatarFallback } from "@shiron/ui/components/ui/avatar";
import { Badge } from "@shiron/ui/components/ui/badge";
import { Separator } from "@shiron/ui/components/ui/separator";
import {
	WarningCircle,
	CheckCircle,
	ChatCircle,
	Clock,
	GitBranch,
	Tag,
} from "@phosphor-icons/react";

const comments = [
	{
		id: "c1",
		author: "kuro",
		avatar: "K",
		time: "1h ago",
		body: "I've reproduced this issue locally. It seems to be related to how we parse multi-value headers. The Content-Length header gets corrupted when transfer-encoding is chunked.",
	},
	{
		id: "c2",
		author: "shiron",
		avatar: "S",
		time: "45m ago",
		body: "Good catch! I think the issue is in `HeaderParser::parse_multi()`. We should add a length check before the vector push. Let me push a fix.",
	},
	{
		id: "c3",
		author: "midori",
		avatar: "M",
		time: "20m ago",
		body: "I've written a regression test for this. We should also add fuzzing for the header parsing module.",
	},
];

export function IssueDetail() {
	return (
		<div className="grid grid-cols-1 lg:grid-cols-[1fr_280px] gap-6">
			<div className="space-y-4">
				<div>
					<h1 className="text-lg font-bold mb-1">
						API gateway crashes on malformed request headers
						<span className="text-muted-foreground ml-2 font-normal">#128</span>
					</h1>
					<div className="flex items-center gap-2 text-xs text-muted-foreground">
						<Badge variant="secondary" className="gap-1 text-[10px]">
							<WarningCircle size={11} className="text-green-500" />
							Open
						</Badge>
						<span>
							<span className="font-medium text-foreground">kuro</span> opened
							this issue 1h ago
						</span>
						<span className="flex items-center gap-1">
							<ChatCircle size={11} /> 3 comments
						</span>
					</div>
				</div>

				<Card className="glass border-border/50 p-4">
					<p className="text-sm text-muted-foreground leading-relaxed">
						The API gateway process crashes with a segfault when receiving
						requests with malformed headers. Specifically, when the
						<code className="mx-1 rounded bg-muted px-1.5 py-0.5 text-xs font-mono">
							Content-Length
						</code>
						header contains non-numeric characters mixed with valid digits.
					</p>
					<p className="text-sm text-muted-foreground leading-relaxed mt-3">
						Steps to reproduce:
					</p>
					<ol className="text-sm text-muted-foreground leading-relaxed list-decimal list-inside space-y-1 mt-1">
						<li>Start the gateway with default config</li>
						<li>
							Send a request with header{" "}
							<code className="rounded bg-muted px-1.5 py-0.5 text-xs font-mono">
								Content-Length: 100abc
							</code>
						</li>
						<li>Observe crash in the parser thread</li>
					</ol>
				</Card>

				<Separator />

				<div className="space-y-4">
					{comments.map((comment) => (
						<div key={comment.id} className="flex gap-3">
							<Avatar className="size-7 mt-0.5 shrink-0">
								<AvatarFallback className="text-[10px] bg-primary/10 text-primary">
									{comment.avatar}
								</AvatarFallback>
							</Avatar>
							<div className="flex-1 space-y-1">
								<div className="flex items-center gap-2">
									<span className="text-xs font-semibold">
										{comment.author}
									</span>
									<span className="text-[11px] text-muted-foreground">
										{comment.time}
									</span>
								</div>
								<div className="glass rounded-lg border border-border/50 p-3">
									<p className="text-sm text-foreground/80 leading-relaxed">
										{comment.body}
									</p>
								</div>
							</div>
						</div>
					))}
				</div>
			</div>

			<aside className="space-y-4">
				<div className="glass rounded-xl border border-border/50 p-4 space-y-3">
					<h4 className="text-[11px] font-medium text-muted-foreground uppercase tracking-wider">
						Details
					</h4>
					{[
						{
							icon: CheckCircle,
							label: "Assignee",
							value: "shiron",
						},
						{ icon: Tag, label: "Labels", value: "bug, critical" },
						{
							icon: GitBranch,
							label: "Milestone",
							value: "v0.2.0",
						},
						{
							icon: Clock,
							label: "Due date",
							value: "Apr 15, 2026",
						},
					].map((item) => {
						const Icon = item.icon;
						return (
							<div
								key={item.label}
								className="flex items-center justify-between text-xs"
							>
								<span className="flex items-center gap-1.5 text-muted-foreground">
									<Icon size={12} />
									{item.label}
								</span>
								<span className="font-medium">{item.value}</span>
							</div>
						);
					})}
				</div>
			</aside>
		</div>
	);
}
