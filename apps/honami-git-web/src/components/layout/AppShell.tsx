import { useState } from "react";
import { SidebarProvider } from "@shiron/ui/components/ui/sidebar";
import { DensityProvider } from "@/components/shared/DensityProvider";
import { TopBar } from "@/components/layout/TopBar";
import { LeftRail } from "@/components/layout/LeftRail";
import { CommandPalette } from "@/components/shared/CommandPalette";
import { NewRepoDialog } from "@/components/repository/NewRepoDialog";

export function AppShell({ children }: { children: React.ReactNode }) {
	const [, setCommandOpen] = useState(false);
	const [newRepoOpen, setNewRepoOpen] = useState(false);

	return (
		<DensityProvider>
			<SidebarProvider>
				<div className="flex h-screen w-full flex-col bg-background">
					<TopBar
						onCommandPalette={() => setCommandOpen(true)}
						onNewRepo={() => setNewRepoOpen(true)}
					/>
					<div className="flex flex-1 overflow-hidden">
						<LeftRail />
						<main className="flex-1 overflow-y-auto">{children}</main>
					</div>
				</div>
				<CommandPalette />
				<NewRepoDialog open={newRepoOpen} onOpenChange={setNewRepoOpen} />
			</SidebarProvider>
		</DensityProvider>
	);
}
