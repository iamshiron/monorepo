import type { Attribute } from "@/api/model/attribute";

export const ATTRIBUTE_MAP = {
	0: "Fusion",
	1: "Glacio",
	2: "Aero",
	3: "Electro",
	4: "Spectro",
	5: "Havoc",
} as const satisfies Record<Attribute, string>;

export const ATTRIBUTE_ENTRIES = Object.entries(ATTRIBUTE_MAP).map(
	([key, value]) => [Number(key) as Attribute, value] as const,
);

export function getAttributeName(attribute: Attribute): string {
	return ATTRIBUTE_MAP[attribute as keyof typeof ATTRIBUTE_MAP];
}
