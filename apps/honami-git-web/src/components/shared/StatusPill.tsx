import { Badge } from "@shiron/ui/components/ui/badge";
import {
	CheckCircle,
	Circle,
	GitMerge,
	Clock,
	Warning,
	XCircle,
} from "@phosphor-icons/react";

type Status =
	| "open"
	| "closed"
	| "merged"
	| "draft"
	| "running"
	| "failed"
	| "success"
	| "warning";

const statusConfig: Record<
	Status,
	{
		label: string;
		variant: "default" | "secondary" | "destructive" | "outline";
		icon: React.ElementType;
	}
> = {
	open: { label: "Open", variant: "secondary", icon: Circle },
	closed: { label: "Closed", variant: "destructive", icon: XCircle },
	merged: { label: "Merged", variant: "default", icon: GitMerge },
	draft: { label: "Draft", variant: "outline", icon: Clock },
	running: { label: "Running", variant: "secondary", icon: Clock },
	failed: { label: "Failed", variant: "destructive", icon: XCircle },
	success: { label: "Passed", variant: "secondary", icon: CheckCircle },
	warning: { label: "Warning", variant: "outline", icon: Warning },
};

export function StatusPill({ status }: { status: Status }) {
	const config = statusConfig[status];
	const Icon = config.icon;
	return (
		<Badge variant={config.variant} className="gap-1">
			<Icon size={12} weight="fill" />
			{config.label}
		</Badge>
	);
}
