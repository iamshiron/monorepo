import { useRef, useEffect } from "react";
import { PaperPlaneTilt } from "@phosphor-icons/react";
import { Button } from "@shiron/ui/components/ui/button";
import { Textarea } from "@shiron/ui/components/ui/textarea";
import { ScrollArea } from "@shiron/ui/components/ui/scroll-area";
import { Avatar, AvatarFallback } from "@shiron/ui/components/ui/avatar";
import { Separator } from "@shiron/ui/components/ui/separator";
import { MOCK_MESSAGES, MOCK_PARTICIPANTS } from "./mock-data";
import { getInitials } from "./ChatListItem";

export function ChatMessageArea() {
	const scrollRef = useRef<HTMLDivElement>(null);

	useEffect(() => {
		if (scrollRef.current) {
			const viewport = scrollRef.current.querySelector(
				"[data-slot=scroll-area-viewport]",
			);
			if (viewport) {
				viewport.scrollTop = viewport.scrollHeight;
			}
		}
	}, []);

	return (
		<div className="flex flex-col h-full min-h-0">
			<ScrollArea className="flex-1 min-h-0 h-full" ref={scrollRef}>
				<div className="max-w-3xl mx-auto px-4 py-4 space-y-1">
					{MOCK_MESSAGES.map((msg, idx) => {
						const isUser = msg.senderType === "user";
						const participant = isUser
							? MOCK_PARTICIPANTS.users[0]
							: MOCK_PARTICIPANTS.agents.find(
									(a) => a.agentId === msg.senderId,
								);

						return (
							<div key={msg.id} className="space-y-1">
								{idx === 0 && <Separator />}
								<div
									className={`flex gap-2.5 py-3 ${
										isUser ? "flex-row-reverse" : ""
									}`}
								>
									<Avatar size="sm" className="shrink-0 mt-0.5">
										<AvatarFallback
											className={`text-[10px] ${
												isUser ? "" : "bg-primary/10 text-primary"
											}`}
										>
											{getInitials(msg.senderName)}
										</AvatarFallback>
									</Avatar>
									<div
										className={`flex flex-col gap-1 max-w-[75%] ${
											isUser ? "items-end" : ""
										}`}
									>
										<div className="flex items-baseline gap-2">
											<span className="text-[11px] font-medium">
												{msg.senderName}
											</span>
											{!isUser &&
												participant &&
												"persona" in participant &&
												participant.persona && (
													<span className="text-[9px] text-muted-foreground">
														{participant.persona.traits?.[0]}
													</span>
												)}
										</div>
										<div
											className={`rounded-xl px-3 py-2 text-xs leading-relaxed ${
												isUser
													? "bg-primary text-primary-foreground"
													: "bg-muted"
											}`}
										>
											{msg.content}
										</div>
										<span className="text-[9px] text-muted-foreground">
											{new Date(msg.timestamp).toLocaleTimeString([], {
												hour: "2-digit",
												minute: "2-digit",
											})}
										</span>
									</div>
								</div>
								{idx < MOCK_MESSAGES.length - 1 && <Separator />}
							</div>
						);
					})}
				</div>
			</ScrollArea>

			<Separator />

			<div className="p-3">
				<div className="max-w-3xl mx-auto flex items-end gap-2">
					<Textarea
						placeholder="Type a message..."
						className="min-h-10 max-h-32 resize-none flex-1"
						rows={1}
						disabled
					/>
					<Button variant="default" size="icon" disabled>
						<PaperPlaneTilt size={16} />
					</Button>
				</div>
			</div>
		</div>
	);
}
