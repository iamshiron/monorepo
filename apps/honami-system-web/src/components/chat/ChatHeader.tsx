import { Users } from "@phosphor-icons/react";
import { Button } from "@shiron/ui/components/ui/button";
import { Separator } from "@shiron/ui/components/ui/separator";
import { SidebarTrigger } from "@shiron/ui/components/ui/sidebar";
import { ModeToggle } from "@/components/layout/ModeToggle";
import { useGetChatParticipants } from "@/api/chat/chat";
import { useIsMobile } from "@shiron/ui/hooks/use-mobile";

interface ChatHeaderProps {
	chatID: string | undefined;
	onToggleRightPanel: () => void;
	rightPanelOpen: boolean;
}

export function ChatHeader({
	chatID,
	onToggleRightPanel,
	rightPanelOpen,
}: ChatHeaderProps) {
	const { data: participants } = useGetChatParticipants(chatID ?? "", {
		query: { enabled: !!chatID },
	});
	const isMobile = useIsMobile();

	const agentCount = participants?.agents?.length ?? 0;

	let subtitle: string | null = null;
	if (chatID && participants) {
		if (agentCount === 1) {
			subtitle = `with ${participants.agents[0].name}`;
		} else if (agentCount > 1) {
			subtitle = `${agentCount} agents`;
		}
	}

	return (
		<header className="flex items-center gap-2 px-3 h-13 border-b bg-background/95 backdrop-blur-md shrink-0">
			<SidebarTrigger />
			<Separator orientation="vertical" className="h-5" />
			<div className="flex-1 min-w-0">
				<div className="text-xs font-medium truncate leading-tight">
					{chatID ? "Chat" : "Honami Chat"}
				</div>
				{subtitle && (
					<div className="text-[10px] text-muted-foreground truncate leading-tight">
						{subtitle}
					</div>
				)}
			</div>
			{chatID && (
				<Button
					variant={rightPanelOpen ? "secondary" : "ghost"}
					size="icon-xs"
					onClick={onToggleRightPanel}
				>
					<Users size={16} />
				</Button>
			)}
			{isMobile && <ModeToggle />}
		</header>
	);
}
