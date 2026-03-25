import { useState } from "react";
import {
	AlertDialog,
	AlertDialogAction,
	AlertDialogCancel,
	AlertDialogContent,
	AlertDialogDescription,
	AlertDialogFooter,
	AlertDialogHeader,
	AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import type { CollectionEntry } from "@/types";

export function DeleteConfirmModal({
	entry,
	isOpen,
	onClose,
	onConfirm,
}: {
	entry: CollectionEntry | null;
	isOpen: boolean;
	onClose: () => void;
	onConfirm: (id: string) => Promise<void>;
}) {
	const [isDeleting, setIsDeleting] = useState(false);

	const handleConfirm = async () => {
		if (!entry) return;
		setIsDeleting(true);
		try {
			await onConfirm(entry.id);
			onClose();
		} finally {
			setIsDeleting(false);
		}
	};

	return (
		<AlertDialog
			open={isOpen}
			onOpenChange={(open) => {
				if (!open) onClose();
			}}
		>
			<AlertDialogContent>
				<AlertDialogHeader>
					<AlertDialogTitle>Remove Character</AlertDialogTitle>
					<AlertDialogDescription>
						Are you sure you want to remove{" "}
						<span className="text-foreground font-semibold">
							{entry?.character.name}
						</span>{" "}
						from your collection?
					</AlertDialogDescription>
				</AlertDialogHeader>
				<AlertDialogFooter>
					<AlertDialogCancel onClick={onClose}>Cancel</AlertDialogCancel>
					<AlertDialogAction onClick={handleConfirm} disabled={isDeleting}>
						{isDeleting ? "Removing..." : "Remove"}
					</AlertDialogAction>
				</AlertDialogFooter>
			</AlertDialogContent>
		</AlertDialog>
	);
}
