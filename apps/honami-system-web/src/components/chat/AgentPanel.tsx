import { useState } from "react";
import { CaretDown, CaretRight, Robot } from "@phosphor-icons/react";
import { Button } from "@shiron/ui/components/ui/button";
import { Avatar, AvatarFallback } from "@shiron/ui/components/ui/avatar";
import { Badge } from "@shiron/ui/components/ui/badge";
import {
	Collapsible,
	CollapsibleContent,
	CollapsibleTrigger,
} from "@shiron/ui/components/ui/collapsible";
import { Separator } from "@shiron/ui/components/ui/separator";
import type { AgentResponse } from "@/api/model";

interface AgentPanelProps {
	agent: AgentResponse;
}

export function AgentPanel({ agent }: AgentPanelProps) {
	const [open, setOpen] = useState(false);
	const initials = agent.name
		.split(" ")
		.map((w) => w[0])
		.join("")
		.slice(0, 2)
		.toUpperCase();

	return (
		<Collapsible open={open} onOpenChange={setOpen}>
			<div className="flex items-center gap-2.5 px-3 py-2 rounded-md hover:bg-muted/50 transition-colors">
				<Avatar size="sm">
					<AvatarFallback className="bg-primary/10 text-primary text-[10px]">
						{initials}
					</AvatarFallback>
				</Avatar>
				<span className="text-xs font-medium truncate flex-1">
					{agent.name}
				</span>
				<CollapsibleTrigger asChild>
					<Button variant="ghost" size="icon-xs">
						{open ? <CaretDown size={14} /> : <CaretRight size={14} />}
					</Button>
				</CollapsibleTrigger>
			</div>
			<CollapsibleContent>
				<div className="pl-9 pr-3 pb-3 space-y-3">
					{agent.description && (
						<p className="text-[11px] text-muted-foreground leading-relaxed">
							{agent.description}
						</p>
					)}

					{agent.persona && (
						<div className="space-y-1.5">
							<div className="flex items-center gap-1.5">
								<Robot size={12} className="text-muted-foreground" />
								<span className="text-[11px] font-medium">
									{agent.persona.name}
								</span>
							</div>
							{agent.persona.brief && (
								<p className="text-[11px] text-muted-foreground leading-relaxed pl-[18px]">
									{agent.persona.brief}
								</p>
							)}
							{agent.persona.speakingStyle && (
								<div className="pl-[18px]">
									<Badge variant="secondary" className="text-[9px]">
										{agent.persona.speakingStyle}
									</Badge>
								</div>
							)}
						</div>
					)}

					{(agent.requiredTools.length > 0 ||
						agent.suggestedTools.length > 0) && (
						<>
							<Separator />
							<div className="space-y-1.5">
								<span className="text-[10px] font-medium text-muted-foreground uppercase tracking-wider">
									Tools
								</span>
								<div className="flex flex-wrap gap-1">
									{agent.requiredTools.map((tool) => (
										<Badge
											key={tool}
											variant="default"
											className="text-[9px] px-1"
										>
											{tool}
										</Badge>
									))}
									{agent.suggestedTools.map((tool) => (
										<Badge
											key={tool}
											variant="outline"
											className="text-[9px] px-1"
										>
											{tool}
										</Badge>
									))}
								</div>
							</div>
						</>
					)}
				</div>
			</CollapsibleContent>
		</Collapsible>
	);
}
