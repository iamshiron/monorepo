import { Copy, Check, Terminal } from "@phosphor-icons/react";
import { useState } from "react";
import { Tabs, TabsList, TabsTrigger } from "@shiron/ui/components/ui/tabs";

function CopyBlock({ lines }: { lines: string[] }) {
	const [copied, setCopied] = useState(false);
	const text = lines.join("\n");

	const handleCopy = () => {
		navigator.clipboard.writeText(text);
		setCopied(true);
		setTimeout(() => setCopied(false), 2000);
	};

	const lineEntries = lines.map((content, i) => ({
		id: `ln-${content.slice(0, 12).replace(/\s/g, "_")}-${content.length}-${i}`,
		content,
	}));

	return (
		<div className="rounded-lg bg-muted/30 border border-border/30 overflow-hidden">
			<div className="flex items-center justify-between px-3 py-1.5 border-b border-border/30">
				<div className="flex items-center gap-1.5 text-[11px] text-muted-foreground">
					<Terminal size={11} />
					<span className="font-medium">Terminal</span>
				</div>
				<button
					type="button"
					onClick={handleCopy}
					className="flex items-center gap-1 text-[11px] text-muted-foreground hover:text-foreground transition-colors"
				>
					{copied ? (
						<Check size={11} className="text-green-500" />
					) : (
						<Copy size={11} />
					)}
					{copied ? "Copied" : "Copy"}
				</button>
			</div>
			<pre className="p-3 text-xs font-mono leading-relaxed overflow-x-auto">
				{lineEntries.map((line) => (
					<div key={line.id} className="flex">
						<span className="text-muted-foreground/40 select-none mr-2">$</span>
						<span className="text-foreground/80">{line.content}</span>
					</div>
				))}
			</pre>
		</div>
	);
}

export function EmptyRepo({ owner, repo }: { owner: string; repo: string }) {
	const [method, setMethod] = useState<"ssh" | "https">("ssh");

	const sshUrl = `git@honamigit.dev:${owner}/${repo}.git`;
	const httpsUrl = `https://honamigit.dev/${owner}/${repo}.git`;
	const url = method === "ssh" ? sshUrl : httpsUrl;

	const pushCommands = [
		"git remote add origin " + url,
		"git branch -M main",
		"git push -u origin main",
	];

	const createCommands = [
		`mkdir ${repo}`,
		`cd ${repo}`,
		"git init",
		"touch README.md",
		"git add README.md",
		'git commit -m "Initial commit"',
		"git remote add origin " + url,
		"git branch -M main",
		"git push -u origin main",
	];

	return (
		<div className="flex flex-col items-center py-12 space-y-8 max-w-2xl mx-auto">
			<div className="text-center space-y-2">
				<h3 className="text-sm font-semibold">This repository is empty</h3>
				<p className="text-xs text-muted-foreground max-w-sm">
					Get started by pushing an existing repository from the command line or
					create a new one.
				</p>
			</div>

			<div className="w-full space-y-6">
				<div className="space-y-3">
					<h4 className="text-xs font-semibold">
						Push an existing repository from the command line
					</h4>
					<Tabs
						value={method}
						onValueChange={(v) => setMethod(v as "ssh" | "https")}
					>
						<TabsList className="h-7">
							<TabsTrigger value="ssh" className="text-[11px] h-6 px-3">
								SSH
							</TabsTrigger>
							<TabsTrigger value="https" className="text-[11px] h-6 px-3">
								HTTPS
							</TabsTrigger>
						</TabsList>
					</Tabs>
					<CopyBlock lines={pushCommands} />
				</div>

				<div className="space-y-3">
					<h4 className="text-xs font-semibold">
						Create a new repository from the command line
					</h4>
					<Tabs
						value={method}
						onValueChange={(v) => setMethod(v as "ssh" | "https")}
					>
						<TabsList className="h-7">
							<TabsTrigger value="ssh" className="text-[11px] h-6 px-3">
								SSH
							</TabsTrigger>
							<TabsTrigger value="https" className="text-[11px] h-6 px-3">
								HTTPS
							</TabsTrigger>
						</TabsList>
					</Tabs>
					<CopyBlock lines={createCommands} />
				</div>
			</div>
		</div>
	);
}
