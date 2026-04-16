import { createFileRoute } from "@tanstack/react-router";
import { ChatMessageArea } from "@/components/chat/ChatMessageArea";

export const Route = createFileRoute("/chat/$chatID")({
	component: ChatMessageArea,
});
