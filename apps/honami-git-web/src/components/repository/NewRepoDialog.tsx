import { useState } from "react";
import {
	Dialog,
	DialogContent,
	DialogDescription,
	DialogHeader,
	DialogTitle,
} from "@shiron/ui/components/ui/dialog";
import { Button } from "@shiron/ui/components/ui/button";
import { Input } from "@shiron/ui/components/ui/input";
import { Label } from "@shiron/ui/components/ui/label";
import { Textarea } from "@shiron/ui/components/ui/textarea";
import { Separator } from "@shiron/ui/components/ui/separator";
import { Lock, Globe } from "@phosphor-icons/react";

export function NewRepoDialog({
	open,
	onOpenChange,
}: {
	open: boolean;
	onOpenChange: (open: boolean) => void;
}) {
	const [name, setName] = useState("");
	const [description, setDescription] = useState("");
	const [isPrivate, setIsPrivate] = useState(false);

	const slug = name
		.toLowerCase()
		.replace(/[^a-z0-9]+/g, "-")
		.replace(/^-|-$/g, "");

	return (
		<Dialog open={open} onOpenChange={onOpenChange}>
			<DialogContent className="sm:max-w-lg">
				<DialogHeader>
					<DialogTitle>Create New Repository</DialogTitle>
					<DialogDescription>
						Create a new repository to start collaborating.
					</DialogDescription>
				</DialogHeader>

				<div className="space-y-4 pt-2">
					<div className="space-y-2">
						<Label className="text-xs">Repository name</Label>
						<Input
							placeholder="my-awesome-project"
							value={name}
							onChange={(e) => setName(e.target.value)}
							className="h-8 text-sm"
						/>
						<p className="text-[11px] text-muted-foreground">
							Namespace:{" "}
							<span className="font-mono">shiron/{slug || "..."}</span>
						</p>
					</div>

					<div className="space-y-2">
						<Label className="text-xs">Description (optional)</Label>
						<Textarea
							placeholder="A brief description of your project"
							value={description}
							onChange={(e) => setDescription(e.target.value)}
							className="text-sm min-h-[72px] resize-none"
							rows={3}
						/>
					</div>

					<Separator />

					<div className="space-y-3">
						<Label className="text-xs">Visibility</Label>
						<div className="grid grid-cols-2 gap-2">
							<button
								type="button"
								onClick={() => setIsPrivate(false)}
								className={`flex items-start gap-2.5 rounded-lg border p-3 text-left transition-colors ${!isPrivate ? "border-primary bg-primary/5" : "border-border/50 hover:bg-muted/30"}`}
							>
								<Globe size={16} className="mt-0.5 shrink-0 text-green-500" />
								<div>
									<span className="text-xs font-medium block">Public</span>
									<span className="text-[11px] text-muted-foreground">
										Anyone can see this repository
									</span>
								</div>
							</button>
							<button
								type="button"
								onClick={() => setIsPrivate(true)}
								className={`flex items-start gap-2.5 rounded-lg border p-3 text-left transition-colors ${isPrivate ? "border-primary bg-primary/5" : "border-border/50 hover:bg-muted/30"}`}
							>
								<Lock size={16} className="mt-0.5 shrink-0 text-yellow-500" />
								<div>
									<span className="text-xs font-medium block">Private</span>
									<span className="text-[11px] text-muted-foreground">
										Only you and collaborators
									</span>
								</div>
							</button>
						</div>
					</div>

					<Separator />

					<div className="flex items-center justify-end gap-2">
						<Button
							variant="outline"
							size="sm"
							className="h-8 text-xs"
							onClick={() => onOpenChange(false)}
						>
							Cancel
						</Button>
						<Button size="sm" className="h-8 text-xs" disabled={!name.trim()}>
							Create Repository
						</Button>
					</div>
				</div>
			</DialogContent>
		</Dialog>
	);
}
