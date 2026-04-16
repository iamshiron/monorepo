import { useState } from "react";
import { Outlet, useRouterState } from "@tanstack/react-router";
import { cn } from "@shiron/ui/lib/utils";
import {
	SidebarProvider,
	Sidebar,
	SidebarInset,
	SidebarRail,
} from "@shiron/ui/components/ui/sidebar";
import {
	Sheet,
	SheetContent,
	SheetHeader,
	SheetTitle,
	SheetDescription,
} from "@shiron/ui/components/ui/sheet";
import { useIsMobile } from "@shiron/ui/hooks/use-mobile";
import { ChatSidebar } from "./ChatSidebar";
import { ChatHeader } from "./ChatHeader";
import { ChatRightSidebar } from "./ChatRightSidebar";

function useChatID() {
	return useRouterState({
		select: (s) => {
			const match = s.location.pathname.match(/^\/chat\/([^/]+)/);
			return match?.[1];
		},
	});
}

export function ChatLayout() {
	const [rightPanelOpen, setRightPanelOpen] = useState(false);
	const isMobile = useIsMobile();
	const chatID = useChatID();

	const rightContent = chatID ? <ChatRightSidebar chatID={chatID} /> : null;

	return (
		<SidebarProvider>
			<Sidebar side="left" collapsible="offcanvas">
				<ChatSidebar />
				<SidebarRail />
			</Sidebar>
			<SidebarInset className="flex flex-col h-svh">
				<ChatHeader
					chatID={chatID}
					onToggleRightPanel={() => setRightPanelOpen((p) => !p)}
					rightPanelOpen={rightPanelOpen}
				/>
				<div className="flex flex-1 min-h-0 overflow-hidden">
					<div className="flex flex-col flex-1 min-w-0 min-h-0">
						<Outlet />
					</div>
					{chatID && !isMobile && (
						<div
							className={cn(
								"flex-col border-l bg-background transition-[width] duration-200 overflow-hidden",
								rightPanelOpen ? "flex w-[340px]" : "w-0",
							)}
						>
							<div className="w-[340px] min-w-[340px] h-full">
								{rightContent}
							</div>
						</div>
					)}
				</div>
			</SidebarInset>
			{chatID && isMobile && (
				<Sheet open={rightPanelOpen} onOpenChange={setRightPanelOpen}>
					<SheetContent side="right" className="w-[320px] p-0">
						<SheetHeader className="sr-only">
							<SheetTitle>Chat Details</SheetTitle>
							<SheetDescription>Participants and agents</SheetDescription>
						</SheetHeader>
						{rightContent}
					</SheetContent>
				</Sheet>
			)}
		</SidebarProvider>
	);
}
