export interface MockMessage {
	id: string;
	senderId: string;
	senderName: string;
	senderType: "user" | "agent";
	content: string;
	timestamp: string;
}

export const MOCK_PARTICIPANTS = {
	users: [{ userId: "u-1", name: "You" }],
	agents: [
		{
			agentId: "a-1",
			name: "Luna",
			allowedTools: ["web_search", "memory_recall"],
			persona: {
				id: "p-1",
				name: "Luna — Night Scholar",
				brief:
					"A quiet, erudite companion who speaks in measured prose with occasional poetic turns.",
				speakingStyle:
					"Elegant, contemplative. Uses metaphor and subtle warmth.",
				traits: ["Curious", "Patient", "Wry"],
			},
		},
		{
			agentId: "a-2",
			name: "Kael",
			allowedTools: ["web_search", "code_execute"],
			persona: {
				id: "p-2",
				name: "Kael — Wandering Blade",
				brief:
					"A gruff but loyal mercenary with a dry sense of humor and a surprising love of literature.",
				speakingStyle: "Blunt, economical. Drops unexpected eloquence.",
				traits: ["Loyal", "Sardonic", "Protective"],
			},
		},
	],
};

export const MOCK_MESSAGES: MockMessage[] = [
	{
		id: "m-1",
		senderId: "a-1",
		senderName: "Luna",
		senderType: "agent",
		content:
			"*tilts head, studying the old map spread across the table* This cartography is... imprecise. The mountain pass marked here was sealed after the Ashfall. Whoever drew this hasn't walked these lands in years.",
		timestamp: "2026-04-16T09:12:00Z",
	},
	{
		id: "m-2",
		senderId: "u-1",
		senderName: "You",
		senderType: "user",
		content:
			"Can we find another route? The supply caravan needs to reach the outpost by week's end.",
		timestamp: "2026-04-16T09:13:15Z",
	},
	{
		id: "m-3",
		senderId: "a-2",
		senderName: "Kael",
		senderType: "agent",
		content:
			"*leans against the doorframe, arms crossed* There's the Gorge. Longer, rougher, but open. I've walked it twice — once by choice, once decidedly not. *a pause* The second time was more interesting.",
		timestamp: "2026-04-16T09:14:00Z",
	},
	{
		id: "m-4",
		senderId: "a-1",
		senderName: "Luna",
		senderType: "agent",
		content:
			"The Gorge it is, then. *traces a line with her finger* If we leave at first light and maintain pace, three days should suffice. I'll prepare a revised itinerary — the hot springs near Mile 40 make for a defensible camp.",
		timestamp: "2026-04-16T09:14:30Z",
	},
	{
		id: "m-5",
		senderId: "u-1",
		senderName: "You",
		senderType: "user",
		content: "Hot springs? Sounds like a perk.",
		timestamp: "2026-04-16T09:15:00Z",
	},
	{
		id: "m-6",
		senderId: "a-2",
		senderName: "Kael",
		senderType: "agent",
		content:
			"*snorts* Don't get comfortable. The water's warm, but the wildlife isn't. Saw a crawler there once, big as a cart. Beautiful creature, terrible manners. *checks blade edge* I'll take first watch.",
		timestamp: "2026-04-16T09:15:30Z",
	},
];
