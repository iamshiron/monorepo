import { useState } from "react";
import { Plus } from "@phosphor-icons/react";
import {
	SidebarHeader,
	SidebarContent,
	SidebarFooter,
	SidebarGroup,
	SidebarGroupLabel,
	SidebarGroupContent,
	SidebarMenu,
	SidebarInput,
	SidebarSeparator,
} from "@shiron/ui/components/ui/sidebar";
import {
	Collapsible,
	CollapsibleContent,
	CollapsibleTrigger,
} from "@shiron/ui/components/ui/collapsible";
import { Button } from "@shiron/ui/components/ui/button";
import { ScrollArea } from "@shiron/ui/components/ui/scroll-area";
import { Spinner } from "@shiron/ui/components/ui/spinner";
import { ModeToggle } from "@/components/layout/ModeToggle";
import { ChatListItem } from "./ChatListItem";
import { NewChatDialog } from "./NewChatDialog";
import { useListChats, useListChatGroups } from "@/api/chat/chat";
import { useRouterState } from "@tanstack/react-router";

export function ChatSidebar() {
	const [search, setSearch] = useState("");
	const [newChatOpen, setNewChatOpen] = useState(false);
	const { data: chats, isLoading: chatsLoading } = useListChats();
	const { data: groups } = useListChatGroups();

	const chatID = useRouterState({
		select: (s) => {
			const match = s.location.pathname.match(/^\/chat\/([^/]+)/);
			return match?.[1];
		},
	});

	const filteredChats = (chats ?? []).filter((c) =>
		c.title.toLowerCase().includes(search.toLowerCase()),
	);

	const ungroupedChats = filteredChats.filter((c) => !c.groupId);

	const groupedChats = (groups ?? [])
		.map((group) => ({
			...group,
			chats: filteredChats.filter((c) => c.groupId === group.id),
		}))
		.filter((g) => g.chats.length > 0);

	return (
		<>
			<SidebarHeader className="p-3">
				<div className="flex items-center justify-between">
					<span className="text-sm font-semibold tracking-tight">Chats</span>
					<Button
						variant="ghost"
						size="icon-xs"
						onClick={() => setNewChatOpen(true)}
					>
						<Plus size={16} />
					</Button>
				</div>
				<SidebarInput
					placeholder="Search chats..."
					value={search}
					onChange={(e) => setSearch((e.target as HTMLInputElement).value)}
				/>
			</SidebarHeader>

			<SidebarContent>
				<ScrollArea className="h-full">
					{chatsLoading ? (
						<div className="flex items-center justify-center py-8">
							<Spinner className="size-5" />
						</div>
					) : (
						<>
							{groupedChats.map((group) => (
								<SidebarGroup key={group.id}>
									<Collapsible defaultOpen>
										<SidebarGroupLabel asChild className="group/label">
											<CollapsibleTrigger className="flex w-full items-center gap-1">
												<span className="flex-1 truncate text-left">
													{group.name}
												</span>
												<span className="text-[9px] text-muted-foreground group-data-[state=open]/label:rotate-90 transition-transform">
													▶
												</span>
											</CollapsibleTrigger>
										</SidebarGroupLabel>
										<CollapsibleContent>
											<SidebarGroupContent>
												<SidebarMenu>
													{group.chats.map((chat) => (
														<ChatListItem
															key={chat.id}
															chat={chat}
															isActive={chat.id === chatID}
														/>
													))}
												</SidebarMenu>
											</SidebarGroupContent>
										</CollapsibleContent>
									</Collapsible>
								</SidebarGroup>
							))}

							{ungroupedChats.length > 0 && (
								<SidebarGroup>
									<SidebarGroupLabel>Recent</SidebarGroupLabel>
									<SidebarGroupContent>
										<SidebarMenu>
											{ungroupedChats.map((chat) => (
												<ChatListItem
													key={chat.id}
													chat={chat}
													isActive={chat.id === chatID}
												/>
											))}
										</SidebarMenu>
									</SidebarGroupContent>
								</SidebarGroup>
							)}

							{filteredChats.length === 0 && !chatsLoading && (
								<div className="px-4 py-8 text-center">
									<p className="text-xs text-muted-foreground">
										{search ? "No chats match your search" : "No chats yet"}
									</p>
								</div>
							)}
						</>
					)}
				</ScrollArea>
			</SidebarContent>

			<SidebarFooter>
				<SidebarSeparator />
				<div className="flex items-center justify-between px-1">
					<span className="text-[10px] text-muted-foreground">
						HonamiSystem
					</span>
					<ModeToggle />
				</div>
			</SidebarFooter>

			<NewChatDialog open={newChatOpen} onOpenChange={setNewChatOpen} />
		</>
	);
}
