import { Avatar, AvatarFallback } from "@shiron/ui/components/ui/avatar";
import { Badge } from "@shiron/ui/components/ui/badge";
import type {
	ParticipantUserResponse,
	ParticipantAgentResponse,
} from "@/api/model";

interface ParticipantItemUserProps {
	participant: ParticipantUserResponse;
	isCurrentUser?: boolean;
}

export function ParticipantItemUser({
	participant,
	isCurrentUser,
}: ParticipantItemUserProps) {
	const initials = participant.name
		.split(" ")
		.map((w) => w[0])
		.join("")
		.slice(0, 2)
		.toUpperCase();

	return (
		<div className="flex items-center gap-2.5 px-3 py-2 rounded-md hover:bg-muted/50 transition-colors">
			<Avatar size="sm">
				<AvatarFallback>{initials}</AvatarFallback>
			</Avatar>
			<span className="text-xs font-medium truncate flex-1">
				{participant.name}
			</span>
			{isCurrentUser && (
				<Badge variant="secondary" className="text-[9px]">
					You
				</Badge>
			)}
		</div>
	);
}

interface ParticipantItemAgentProps {
	participant: ParticipantAgentResponse;
}

export function ParticipantItemAgent({
	participant,
}: ParticipantItemAgentProps) {
	const initials = participant.name
		.split(" ")
		.map((w) => w[0])
		.join("")
		.slice(0, 2)
		.toUpperCase();

	return (
		<div className="flex items-center gap-2.5 px-3 py-2 rounded-md hover:bg-muted/50 transition-colors">
			<Avatar size="sm">
				<AvatarFallback className="bg-primary/10 text-primary text-[10px]">
					{initials}
				</AvatarFallback>
			</Avatar>
			<span className="text-xs font-medium truncate flex-1">
				{participant.name}
			</span>
			{participant.allowedTools.length > 0 && (
				<Badge variant="outline" className="text-[9px] px-1">
					{participant.allowedTools.length} tools
				</Badge>
			)}
		</div>
	);
}
