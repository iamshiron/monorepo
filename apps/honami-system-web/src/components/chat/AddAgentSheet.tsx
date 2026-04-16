import { useState } from "react";
import {
	Sheet,
	SheetContent,
	SheetHeader,
	SheetTitle,
	SheetDescription,
} from "@shiron/ui/components/ui/sheet";
import { Button } from "@shiron/ui/components/ui/button";
import { Spinner } from "@shiron/ui/components/ui/spinner";
import { Checkbox } from "@shiron/ui/components/ui/checkbox";
import { Separator } from "@shiron/ui/components/ui/separator";
import { Avatar, AvatarFallback } from "@shiron/ui/components/ui/avatar";
import { ScrollArea } from "@shiron/ui/components/ui/scroll-area";
import { Field, FieldLabel } from "@shiron/ui/components/ui/field";
import { useListAgents } from "@/api/agents/agents";
import { useGetApiPlugins } from "@/api/plugins/plugins";
import { useAddChatAgentParticipant } from "@/api/chat/chat";

interface AddAgentSheetProps {
	chatID: string;
	open: boolean;
	onOpenChange: (open: boolean) => void;
	existingAgentIds: Set<string>;
}

export function AddAgentSheet({
	chatID,
	open,
	onOpenChange,
	existingAgentIds,
}: AddAgentSheetProps) {
	const [selectedAgentId, setSelectedAgentId] = useState<string | null>(null);
	const [selectedTools, setSelectedTools] = useState<string[]>([]);
	const { data: agents, isLoading: agentsLoading } = useListAgents();
	const { data: plugins } = useGetApiPlugins();
	const addAgent = useAddChatAgentParticipant();

	const availableAgents = (agents ?? []).filter(
		(a) => !existingAgentIds.has(a.id),
	);

	const selectedAgent = availableAgents.find((a) => a.id === selectedAgentId);

	const toggleTool = (tool: string) => {
		setSelectedTools((prev) =>
			prev.includes(tool) ? prev.filter((t) => t !== tool) : [...prev, tool],
		);
	};

	const handleSubmit = () => {
		if (!selectedAgentId) return;
		addAgent.mutate(
			{
				chatId: chatID,
				data: {
					agentId: selectedAgentId,
					allowedTools: selectedTools.length > 0 ? selectedTools : undefined,
				},
			},
			{
				onSuccess: () => {
					setSelectedAgentId(null);
					setSelectedTools([]);
					onOpenChange(false);
				},
			},
		);
	};

	return (
		<Sheet open={open} onOpenChange={onOpenChange}>
			<SheetContent side="right" className="w-[360px] p-0">
				<SheetHeader className="p-4 pb-0">
					<SheetTitle>Add Agent</SheetTitle>
					<SheetDescription>
						Select an agent to add to this conversation
					</SheetDescription>
				</SheetHeader>

				<ScrollArea className="h-[calc(100vh-8rem)]">
					<div className="p-4 space-y-4">
						{agentsLoading ? (
							<div className="flex justify-center py-8">
								<Spinner className="size-5" />
							</div>
						) : availableAgents.length === 0 ? (
							<div className="py-8 text-center">
								<p className="text-xs text-muted-foreground">
									All your agents are already in this chat
								</p>
							</div>
						) : (
							<div className="space-y-1">
								{availableAgents.map((agent) => {
									const initials = agent.name
										.split(" ")
										.map((w) => w[0])
										.join("")
										.slice(0, 2)
										.toUpperCase();
									const isSelected = agent.id === selectedAgentId;

									return (
										<button
											type="button"
											key={agent.id}
											onClick={() => {
												setSelectedAgentId(isSelected ? null : agent.id);
												setSelectedTools([]);
											}}
											className={`flex w-full items-center gap-2.5 rounded-md px-3 py-2.5 text-left transition-colors ${
												isSelected
													? "bg-primary/10 ring-1 ring-primary/20"
													: "hover:bg-muted/50"
											}`}
										>
											<Avatar size="sm">
												<AvatarFallback className="bg-primary/10 text-primary text-[10px]">
													{initials}
												</AvatarFallback>
											</Avatar>
											<div className="flex-1 min-w-0">
												<span className="text-xs font-medium block truncate">
													{agent.name}
												</span>
												{agent.description && (
													<span className="text-[10px] text-muted-foreground block truncate">
														{agent.description}
													</span>
												)}
											</div>
										</button>
									);
								})}
							</div>
						)}

						{selectedAgent && plugins && plugins.length > 0 && (
							<>
								<Separator />
								<div className="space-y-3">
									<span className="text-[10px] font-medium text-muted-foreground uppercase tracking-wider">
										Allowed Tools
									</span>
									<div className="space-y-2">
										{plugins.map((plugin) => (
											<Field key={plugin.triple} orientation="horizontal">
												<Checkbox
													checked={selectedTools.includes(plugin.triple)}
													onCheckedChange={() => toggleTool(plugin.triple)}
												/>
												<FieldLabel className="text-xs font-normal">
													{plugin.name}
													<span className="text-muted-foreground ml-1">
														v{plugin.version}
													</span>
												</FieldLabel>
											</Field>
										))}
									</div>
								</div>
							</>
						)}

						<Button
							className="w-full"
							size="sm"
							disabled={!selectedAgentId || addAgent.isPending}
							onClick={handleSubmit}
						>
							{addAgent.isPending ? (
								<Spinner className="size-4" />
							) : (
								"Add to Chat"
							)}
						</Button>
					</div>
				</ScrollArea>
			</SheetContent>
		</Sheet>
	);
}
