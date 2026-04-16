import { Users, Robot } from "@phosphor-icons/react";
import {
	Tabs,
	TabsList,
	TabsTrigger,
	TabsContent,
} from "@shiron/ui/components/ui/tabs";
import { ScrollArea } from "@shiron/ui/components/ui/scroll-area";
import { Separator } from "@shiron/ui/components/ui/separator";
import { Button } from "@shiron/ui/components/ui/button";
import { Spinner } from "@shiron/ui/components/ui/spinner";
import { Plus } from "@phosphor-icons/react";
import { ParticipantItemUser, ParticipantItemAgent } from "./ParticipantItem";
import { AgentPanel } from "./AgentPanel";
import { AddAgentSheet } from "./AddAgentSheet";
import { useGetChat, useGetChatParticipants } from "@/api/chat/chat";
import { useListAgents } from "@/api/agents/agents";
import { useState } from "react";

interface ChatRightSidebarProps {
	chatID: string;
}

export function ChatRightSidebar({ chatID }: ChatRightSidebarProps) {
	const [addAgentOpen, setAddAgentOpen] = useState(false);
	const { data: chat, isLoading: chatLoading } = useGetChat(chatID);
	const { data: participants, isLoading: participantsLoading } =
		useGetChatParticipants(chatID);
	const { data: allAgents } = useListAgents();

	const existingAgentIds = new Set(
		(participants?.agents ?? []).map((a) => a.agentId),
	);

	const agentsWithDetails = (participants?.agents ?? [])
		.map((p) => {
			const full = allAgents?.find((a) => a.id === p.agentId);
			return full ?? null;
		})
		.filter(Boolean);

	if (chatLoading || participantsLoading) {
		return (
			<div className="flex items-center justify-center h-full">
				<Spinner className="size-5" />
			</div>
		);
	}

	return (
		<div className="flex flex-col h-full">
			<div className="px-4 pt-4 pb-2">
				<h3 className="text-sm font-semibold truncate">
					{chat?.title ?? "Chat Details"}
				</h3>
				{chat?.description && (
					<p className="text-[11px] text-muted-foreground mt-0.5 truncate">
						{chat.description}
					</p>
				)}
			</div>

			<Separator />

			<Tabs
				defaultValue="participants"
				className="flex-1 flex flex-col min-h-0"
			>
				<div className="px-3 pt-2">
					<TabsList className="w-full">
						<TabsTrigger value="participants" className="flex-1 gap-1.5">
							<Users size={13} />
							Participants
						</TabsTrigger>
						<TabsTrigger value="agents" className="flex-1 gap-1.5">
							<Robot size={13} />
							Agents
						</TabsTrigger>
					</TabsList>
				</div>

				<TabsContent value="participants" className="flex-1 min-h-0 mt-0">
					<ScrollArea className="h-full">
						<div className="p-3 space-y-1">
							{(participants?.users ?? []).map((user) => (
								<ParticipantItemUser key={user.userId} participant={user} />
							))}
							{(participants?.agents ?? []).map((agent) => (
								<ParticipantItemAgent key={agent.agentId} participant={agent} />
							))}
						</div>
					</ScrollArea>
				</TabsContent>

				<TabsContent value="agents" className="flex-1 min-h-0 mt-0">
					<ScrollArea className="h-full">
						<div className="p-3 space-y-1">
							{agentsWithDetails.map((agent) =>
								agent ? <AgentPanel key={agent.id} agent={agent} /> : null,
							)}
							{agentsWithDetails.length === 0 && (
								<div className="py-6 text-center">
									<p className="text-xs text-muted-foreground">
										No agents in this chat
									</p>
								</div>
							)}
						</div>
					</ScrollArea>
					<div className="p-3 pt-0">
						<Separator className="mb-3" />
						<Button
							variant="outline"
							size="sm"
							className="w-full gap-1.5"
							onClick={() => setAddAgentOpen(true)}
						>
							<Plus size={14} />
							Add Agent
						</Button>
					</div>
				</TabsContent>
			</Tabs>

			<AddAgentSheet
				chatID={chatID}
				open={addAgentOpen}
				onOpenChange={setAddAgentOpen}
				existingAgentIds={existingAgentIds}
			/>
		</div>
	);
}
