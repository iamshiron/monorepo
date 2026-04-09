import { BookOpen } from "@phosphor-icons/react";

export function ReadmePreview() {
	return (
		<div className="rounded-lg border border-border/50 overflow-hidden">
			<div className="flex items-center gap-2 px-4 py-2 bg-muted/20 border-b border-border/50">
				<BookOpen size={14} className="text-muted-foreground" />
				<span className="text-xs font-medium text-muted-foreground">
					README.md
				</span>
			</div>
			<div className="prose-sm px-6 py-5 max-w-none">
				<h1 className="text-xl font-bold tracking-tight text-foreground mb-1">
					HonamiGit
				</h1>
				<p className="text-muted-foreground text-sm leading-relaxed mb-6">
					A modern Git hosting platform with a focus on beautiful UI, powerful
					CI/CD integration, and developer experience. Built with Rust for the
					backend and React for the frontend.
				</p>

				<div className="flex items-center gap-3 mb-6">
					<BadgePill
						label="Rust"
						className="bg-orange-500/15 text-orange-500"
					/>
					<BadgePill label="React" className="bg-cyan-500/15 text-cyan-500" />
					<BadgePill
						label="TypeScript"
						className="bg-blue-500/15 text-blue-500"
					/>
					<BadgePill
						label="MIT License"
						className="bg-green-500/15 text-green-500"
					/>
				</div>

				<h2 className="text-sm font-semibold text-foreground mb-3 border-b border-border/40 pb-1">
					Features
				</h2>
				<ul className="text-sm text-muted-foreground leading-relaxed mb-6 space-y-1.5 list-disc list-inside">
					<li>Fast and intuitive web interface with glass design</li>
					<li>Built-in CI/CD pipelines with real-time logging</li>
					<li>Code review with inline comments and suggestions</li>
					<li>Repository groups with granular permissions</li>
					<li>Webhook integrations and API access</li>
					<li>Self-hosted or cloud deployment</li>
				</ul>

				<h2 className="text-sm font-semibold text-foreground mb-3 border-b border-border/40 pb-1">
					Getting Started
				</h2>
				<p className="text-sm text-muted-foreground mb-3">
					Clone the repository and build from source:
				</p>
				<div className="rounded-md bg-muted/40 border border-border/30 p-3 mb-6 overflow-x-auto">
					<code className="text-xs font-mono text-foreground/80">
						<span className="text-muted-foreground/60">$</span> git clone
						https://honamigit.dev/shiron/honami-git.git
						<br />
						<span className="text-muted-foreground/60">$</span> cd honami-git
						<br />
						<span className="text-muted-foreground/60">$</span> cargo build
						--release
						<br />
						<span className="text-muted-foreground/60">$</span>{" "}
						./target/release/honami-git serve
					</code>
				</div>

				<h2 className="text-sm font-semibold text-foreground mb-3 border-b border-border/40 pb-1">
					Contributing
				</h2>
				<p className="text-sm text-muted-foreground leading-relaxed mb-3">
					Contributions are welcome! Please read the contributing guidelines
					before submitting merge requests.
				</p>
				<ol className="text-sm text-muted-foreground leading-relaxed mb-6 space-y-1.5 list-decimal list-inside">
					<li>Fork the repository</li>
					<li>
						Create a feature branch (
						<code className="rounded bg-muted/50 px-1.5 py-0.5 text-xs font-mono">
							git checkout -b feature/amazing
						</code>
						)
					</li>
					<li>Commit your changes</li>
					<li>Open a merge request</li>
				</ol>

				<h2 className="text-sm font-semibold text-foreground mb-3 border-b border-border/40 pb-1">
					Documentation
				</h2>
				<p className="text-sm text-muted-foreground leading-relaxed">
					Full documentation is available at{" "}
					<span className="text-primary font-medium">docs.honamigit.dev</span>.
					Covers installation, configuration, API reference, and plugin
					development.
				</p>
			</div>
		</div>
	);
}

function BadgePill({ label, className }: { label: string; className: string }) {
	return (
		<span
			className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-[11px] font-medium ${className}`}
		>
			{label}
		</span>
	);
}
