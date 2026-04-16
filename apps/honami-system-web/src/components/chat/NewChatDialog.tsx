import { useState } from "react";
import {
	Dialog,
	DialogContent,
	DialogHeader,
	DialogTitle,
	DialogDescription,
	DialogFooter,
} from "@shiron/ui/components/ui/dialog";
import { Button } from "@shiron/ui/components/ui/button";
import { Input } from "@shiron/ui/components/ui/input";
import { Textarea } from "@shiron/ui/components/ui/textarea";
import {
	Field,
	FieldLabel,
	FieldDescription,
} from "@shiron/ui/components/ui/field";
import { Spinner } from "@shiron/ui/components/ui/spinner";
import { useCreateChat, getListChatsQueryKey } from "@/api/chat/chat";
import { useNavigate } from "@tanstack/react-router";
import { useQueryClient } from "@tanstack/react-query";

interface NewChatDialogProps {
	open: boolean;
	onOpenChange: (open: boolean) => void;
}

export function NewChatDialog({ open, onOpenChange }: NewChatDialogProps) {
	const [title, setTitle] = useState("");
	const [description, setDescription] = useState("");
	const createChat = useCreateChat();
	const navigate = useNavigate();
	const queryClient = useQueryClient();

	const handleSubmit = () => {
		if (!title.trim()) return;
		createChat.mutate(
			{
				data: {
					title: title.trim(),
					description: description.trim() || undefined,
				},
			},
			{
				onSuccess: (chat) => {
					setTitle("");
					setDescription("");
					onOpenChange(false);
					queryClient.invalidateQueries({
						queryKey: getListChatsQueryKey(),
					});
					navigate({
						to: "/chat/$chatID",
						params: { chatID: chat.id },
					});
				},
			},
		);
	};

	return (
		<Dialog open={open} onOpenChange={onOpenChange}>
			<DialogContent className="sm:max-w-md">
				<DialogHeader>
					<DialogTitle>New Chat</DialogTitle>
					<DialogDescription>
						Start a new conversation or roleplay session
					</DialogDescription>
				</DialogHeader>

				<div className="space-y-4">
					<Field>
						<FieldLabel>Title</FieldLabel>
						<Input
							placeholder="Enter chat title..."
							value={title}
							onChange={(e) => setTitle(e.target.value)}
							maxLength={64}
						/>
						<FieldDescription>{title.length}/64 characters</FieldDescription>
					</Field>

					<Field>
						<FieldLabel>Description</FieldLabel>
						<Textarea
							placeholder="Optional description or scenario..."
							value={description}
							onChange={(e) => setDescription(e.target.value)}
							maxLength={256}
							className="min-h-20"
						/>
						<FieldDescription>
							{description.length}/256 characters
						</FieldDescription>
					</Field>
				</div>

				<DialogFooter>
					<Button variant="outline" onClick={() => onOpenChange(false)}>
						Cancel
					</Button>
					<Button
						onClick={handleSubmit}
						disabled={!title.trim() || createChat.isPending}
					>
						{createChat.isPending ? <Spinner className="size-4" /> : "Create"}
					</Button>
				</DialogFooter>
			</DialogContent>
		</Dialog>
	);
}
