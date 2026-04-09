import { Link, useMatchRoute } from "@tanstack/react-router";
import {
	FolderSimple,
	Users,
	ClockCounterClockwise,
	Star,
	Bookmarks,
	Gear,
	GitBranch,
} from "@phosphor-icons/react";
import {
	Sidebar,
	SidebarContent,
	SidebarGroup,
	SidebarGroupContent,
	SidebarGroupLabel,
	SidebarHeader,
	SidebarMenu,
	SidebarMenuButton,
	SidebarMenuItem,
	SidebarFooter,
	SidebarRail,
} from "@shiron/ui/components/ui/sidebar";
import { Avatar, AvatarFallback } from "@shiron/ui/components/ui/avatar";
import { Badge } from "@shiron/ui/components/ui/badge";

const projects = [
	{ name: "honami-git/core", user: "shiron", repo: "honami-git" },
	{ name: "shiron/empty", user: "shiron", repo: "empty" },
	{ name: "shiron/dotfiles", user: "shiron", repo: "dotfiles" },
	{ name: "org/frontend-kit", user: "org", repo: "frontend-kit" },
];

const groups = [
	{ name: "Shiron Org", user: "shiron", count: 12 },
	{ name: "Open Source", user: "opensource", count: 5 },
];

export function LeftRail() {
	const matchRoute = useMatchRoute();

	return (
		<Sidebar collapsible="icon" className="border-r border-border/50">
			<SidebarHeader className="p-3">
				<div className="flex items-center gap-2 px-2">
					<Avatar className="size-6">
						<AvatarFallback className="text-[10px] bg-primary/10 text-primary font-semibold">
							S
						</AvatarFallback>
					</Avatar>
					<div className="flex flex-col group-data-[collapsible=icon]:hidden">
						<span className="text-xs font-medium leading-tight">Shiron</span>
						<span className="text-[10px] text-muted-foreground">@shiron</span>
					</div>
				</div>
			</SidebarHeader>

			<SidebarContent>
				<SidebarGroup>
					<SidebarGroupLabel className="gap-1.5">
						<Star size={12} className="text-primary" />
						Pinned
					</SidebarGroupLabel>
					<SidebarGroupContent>
						<SidebarMenu>
							{projects.map((project) => (
								<SidebarMenuItem key={project.name}>
									<SidebarMenuButton
										asChild
										isActive={
											!!matchRoute({
												to: "/$user/$repo",
												params: {
													user: project.user,
													repo: project.repo,
												},
											})
										}
										tooltip={project.name}
									>
										<Link
											to="/$user/$repo"
											params={{
												user: project.user,
												repo: project.repo,
											}}
										>
											<GitBranch size={14} />
											<span className="truncate">{project.name}</span>
										</Link>
									</SidebarMenuButton>
								</SidebarMenuItem>
							))}
						</SidebarMenu>
					</SidebarGroupContent>
				</SidebarGroup>

				<SidebarGroup>
					<SidebarGroupLabel className="gap-1.5">
						<Users size={12} />
						Groups
					</SidebarGroupLabel>
					<SidebarGroupContent>
						<SidebarMenu>
							{groups.map((group) => (
								<SidebarMenuItem key={group.name}>
									<SidebarMenuButton asChild tooltip={group.name}>
										<Link to="/$user" params={{ user: group.user }}>
											<FolderSimple size={14} />
											<span className="truncate">{group.name}</span>
											<Badge
												variant="secondary"
												className="ml-auto text-[10px] px-1 py-0 h-4 group-data-[collapsible=icon]:hidden"
											>
												{group.count}
											</Badge>
										</Link>
									</SidebarMenuButton>
								</SidebarMenuItem>
							))}
						</SidebarMenu>
					</SidebarGroupContent>
				</SidebarGroup>

				<SidebarGroup>
					<SidebarGroupLabel className="gap-1.5">
						<ClockCounterClockwise size={12} />
						Recent
					</SidebarGroupLabel>
					<SidebarGroupContent>
						<SidebarMenu>
							<SidebarMenuItem>
								<SidebarMenuButton asChild tooltip="shiron/monorepo">
									<Link
										to="/$user/$repo"
										params={{
											user: "shiron",
											repo: "monorepo",
										}}
									>
										<Bookmarks size={14} />
										<span className="truncate group-data-[collapsible=icon]:hidden">
											shiron/monorepo
										</span>
									</Link>
								</SidebarMenuButton>
							</SidebarMenuItem>
						</SidebarMenu>
					</SidebarGroupContent>
				</SidebarGroup>
			</SidebarContent>

			<SidebarFooter>
				<SidebarMenu>
					<SidebarMenuItem>
						<SidebarMenuButton tooltip="Settings">
							<Gear size={14} />
							<span className="group-data-[collapsible=icon]:hidden">
								Settings
							</span>
						</SidebarMenuButton>
					</SidebarMenuItem>
				</SidebarMenu>
			</SidebarFooter>
			<SidebarRail />
		</Sidebar>
	);
}
