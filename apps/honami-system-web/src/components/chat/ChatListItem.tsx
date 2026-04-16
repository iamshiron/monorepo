import { Link } from "@tanstack/react-router";
import type { ChatResponse } from "@/api/model";
import {
	SidebarMenuButton,
	SidebarMenuItem,
} from "@shiron/ui/components/ui/sidebar";
import { Badge } from "@shiron/ui/components/ui/badge";

interface ChatListItemProps {
	chat: ChatResponse;
	isActive: boolean;
}

function getInitials(name: string) {
	return name
		.split(" ")
		.map((w) => w[0])
		.join("")
		.slice(0, 2)
		.toUpperCase();
}

export function ChatListItem({ chat, isActive }: ChatListItemProps) {
	return (
		<SidebarMenuItem>
			<SidebarMenuButton asChild isActive={isActive} tooltip={chat.title}>
				<Link to="/chat/$chatID" params={{ chatID: chat.id }}>
					<div className="flex size-6 shrink-0 items-center justify-center rounded-md bg-muted text-[10px] font-medium text-muted-foreground">
						{getInitials(chat.title)}
					</div>
					<div className="flex flex-col gap-0.5 overflow-hidden">
						<span className="truncate text-xs font-medium leading-none">
							{chat.title}
						</span>
						{chat.description && (
							<span className="truncate text-[10px] text-muted-foreground leading-none">
								{chat.description}
							</span>
						)}
					</div>
					{chat.groupId && (
						<Badge variant="outline" className="ml-auto text-[9px] px-1">
							group
						</Badge>
					)}
				</Link>
			</SidebarMenuButton>
		</SidebarMenuItem>
	);
}

export { getInitials };
