import { ChatCircleDots } from "@phosphor-icons/react";
import {
	Empty,
	EmptyHeader,
	EmptyMedia,
	EmptyTitle,
	EmptyDescription,
} from "@shiron/ui/components/ui/empty";
import { SidebarTrigger } from "@shiron/ui/components/ui/sidebar";
import { useIsMobile } from "@shiron/ui/hooks/use-mobile";

export function ChatEmptyState() {
	const isMobile = useIsMobile();

	return (
		<div className="flex flex-col items-center justify-center h-full">
			{isMobile && (
				<div className="absolute top-3 left-3 z-10">
					<SidebarTrigger />
				</div>
			)}
			<Empty>
				<EmptyHeader>
					<EmptyMedia variant="icon">
						<ChatCircleDots size={20} />
					</EmptyMedia>
					<EmptyTitle>Select a conversation</EmptyTitle>
					<EmptyDescription>
						Pick a chat from the sidebar or create a new one to start talking
						with your AI companions.
					</EmptyDescription>
				</EmptyHeader>
			</Empty>
		</div>
	);
}
