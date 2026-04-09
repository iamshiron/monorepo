import { useState } from "react";
import {
	Dialog,
	DialogContent,
	DialogHeader,
	DialogTitle,
} from "@shiron/ui/components/ui/dialog";
import { Button } from "@shiron/ui/components/ui/button";
import { Input } from "@shiron/ui/components/ui/input";
import { Separator } from "@shiron/ui/components/ui/separator";
import { Tabs, TabsList, TabsTrigger } from "@shiron/ui/components/ui/tabs";
import { Copy, Check, Download } from "@phosphor-icons/react";

function CopyButton({ text }: { text: string }) {
	const [copied, setCopied] = useState(false);

	const handleCopy = () => {
		navigator.clipboard.writeText(text);
		setCopied(true);
		setTimeout(() => setCopied(false), 2000);
	};

	return (
		<Button
			variant="ghost"
			size="icon"
			className="size-6 shrink-0"
			onClick={handleCopy}
		>
			{copied ? (
				<Check size={12} className="text-green-500" />
			) : (
				<Copy size={12} />
			)}
		</Button>
	);
}

export function CloneDialog({
	open,
	onOpenChange,
	owner,
	repo,
}: {
	open: boolean;
	onOpenChange: (open: boolean) => void;
	owner: string;
	repo: string;
}) {
	const [method, setMethod] = useState<"ssh" | "https">("https");

	const sshUrl = `git@honamigit.dev:${owner}/${repo}.git`;
	const httpsUrl = `https://honamigit.dev/${owner}/${repo}.git`;
	const url = method === "ssh" ? sshUrl : httpsUrl;

	return (
		<Dialog open={open} onOpenChange={onOpenChange}>
			<DialogContent className="sm:max-w-md">
				<DialogHeader>
					<DialogTitle>Clone repository</DialogTitle>
				</DialogHeader>

				<div className="space-y-4 pt-2">
					<Tabs
						value={method}
						onValueChange={(v) => setMethod(v as "ssh" | "https")}
					>
						<TabsList className="h-8">
							<TabsTrigger value="https" className="text-xs h-7 px-3">
								HTTPS
							</TabsTrigger>
							<TabsTrigger value="ssh" className="text-xs h-7 px-3">
								SSH
							</TabsTrigger>
						</TabsList>
					</Tabs>

					<div className="flex items-center gap-1.5">
						<Input readOnly value={url} className="h-8 text-xs font-mono" />
						<CopyButton text={url} />
					</div>

					<p className="text-[11px] text-muted-foreground">
						{method === "https"
							? "Use your username and personal access token when prompted."
							: "Make sure you have an SSH key configured in your account settings."}
					</p>

					<div className="rounded-md bg-muted/30 border border-border/30 p-3">
						<div className="flex items-center justify-between mb-2">
							<span className="text-[11px] font-medium text-muted-foreground">
								Quick setup
							</span>
							<CopyButton text={`git clone ${url}`} />
						</div>
						<code className="text-xs font-mono text-foreground/80 block break-all">
							git clone {url}
						</code>
					</div>

					<Separator />

					<Button
						variant="outline"
						size="sm"
						className="w-full gap-1.5 h-8 text-xs"
					>
						<Download size={14} />
						Download ZIP
					</Button>
				</div>
			</DialogContent>
		</Dialog>
	);
}
