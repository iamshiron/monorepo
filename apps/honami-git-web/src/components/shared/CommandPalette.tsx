import { useEffect, useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import {
	Command,
	CommandDialog,
	CommandInput,
	CommandList,
	CommandEmpty,
	CommandGroup,
	CommandItem,
} from "@shiron/ui/components/ui/command";

const projects = [
	{ name: "honami-git/core", path: "/shiron/honami-git" },
	{ name: "shiron/dotfiles", path: "/shiron/dotfiles" },
	{ name: "shiron/monorepo", path: "/shiron/monorepo" },
	{ name: "org/frontend-kit", path: "/org/frontend-kit" },
	{ name: "org/api-gateway", path: "/org/api-gateway" },
];

const pages = [
	{ name: "Dashboard", path: "/" },
	{ name: "Explore", path: "/explore" },
	{ name: "Your Repositories", path: "/shiron/repositories" },
	{ name: "Profile", path: "/shiron/profile" },
];

const commands = [
	{ name: "Toggle Theme", action: "theme" },
	{ name: "New Repository", action: "new-repo" },
	{ name: "New Issue", action: "new-issue" },
];

export function CommandPalette() {
	const [open, setOpen] = useState(false);
	const navigate = useNavigate();

	useEffect(() => {
		const down = (e: KeyboardEvent) => {
			if (e.key === "k" && (e.metaKey || e.ctrlKey)) {
				e.preventDefault();
				setOpen((o) => !o);
			}
		};
		document.addEventListener("keydown", down);
		return () => document.removeEventListener("keydown", down);
	}, []);

	const runCommand = (command: () => void) => {
		setOpen(false);
		command();
	};

	return (
		<CommandDialog open={open} onOpenChange={setOpen}>
			<Command>
				<CommandInput placeholder="Search repos, pages, or commands..." />
				<CommandList>
					<CommandEmpty>No results found.</CommandEmpty>
					<CommandGroup heading="Pages">
						{pages.map((page) => (
							<CommandItem
								key={page.path}
								onSelect={() => runCommand(() => navigate({ to: page.path }))}
							>
								{page.name}
							</CommandItem>
						))}
					</CommandGroup>
					<CommandGroup heading="Projects">
						{projects.map((project) => (
							<CommandItem
								key={project.path}
								onSelect={() =>
									runCommand(() => navigate({ to: project.path }))
								}
							>
								{project.name}
							</CommandItem>
						))}
					</CommandGroup>
					<CommandGroup heading="Commands">
						{commands.map((cmd) => (
							<CommandItem
								key={cmd.action}
								onSelect={() => runCommand(() => {})}
							>
								{cmd.name}
							</CommandItem>
						))}
					</CommandGroup>
				</CommandList>
			</Command>
		</CommandDialog>
	);
}
