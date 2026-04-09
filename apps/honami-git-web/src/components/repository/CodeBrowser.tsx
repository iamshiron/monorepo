import { Folder, File } from "@phosphor-icons/react";

interface FileEntry {
	name: string;
	type: "file" | "dir";
	lang?: string;
	lastCommit: string;
	time: string;
}

const fileEntries: FileEntry[] = [
	{
		name: "src",
		type: "dir",
		lastCommit: "refactor: extract header component",
		time: "2h ago",
	},
	{
		name: "tests",
		type: "dir",
		lastCommit: "add regression test for header parsing",
		time: "1d ago",
	},
	{
		name: ".gitignore",
		type: "file",
		lastCommit: "ignore build artifacts",
		time: "3d ago",
	},
	{
		name: "Cargo.toml",
		type: "file",
		lastCommit: "bump version to 0.2.0",
		time: "5h ago",
	},
	{
		name: "README.md",
		type: "file",
		lang: "markdown",
		lastCommit: "update installation instructions",
		time: "2d ago",
	},
	{
		name: "package.json",
		type: "file",
		lang: "json",
		lastCommit: "add tailwindcss dependency",
		time: "1w ago",
	},
];

export function CodeBrowser() {
	return (
		<div className="rounded-lg border border-border/50 overflow-hidden">
			<table className="w-full text-xs">
				<tbody>
					{fileEntries.map((entry) => (
						<tr
							key={`file-${entry.name}`}
							className="group border-b border-border/30 last:border-b-0 hover:bg-muted/30 transition-colors"
						>
							<td className="py-2 pl-4 pr-2 w-6">
								{entry.type === "dir" ? (
									<Folder size={15} className="text-blue-400" />
								) : (
									<File size={15} className="text-muted-foreground" />
								)}
							</td>
							<td className="py-2 pr-4 w-48">
								<span
									className={`font-medium ${entry.type === "dir" ? "text-foreground" : "text-foreground/80"}`}
								>
									{entry.name}
								</span>
							</td>
							<td className="py-2 pr-4 hidden sm:table-cell">
								<span className="text-muted-foreground truncate block max-w-xs">
									{entry.lastCommit}
								</span>
							</td>
							<td className="py-2 pr-4 text-right whitespace-nowrap">
								<span className="text-muted-foreground/60">{entry.time}</span>
							</td>
						</tr>
					))}
				</tbody>
			</table>
		</div>
	);
}
