import { createFileRoute } from "@tanstack/react-router";
import { ChatEmptyState } from "@/components/chat/ChatEmptyState";

export const Route = createFileRoute("/chat/")({
	component: ChatEmptyState,
});
