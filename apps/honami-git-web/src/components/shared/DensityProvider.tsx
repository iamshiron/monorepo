import { createContext, useContext, useState } from "react";

type Density = "compact" | "comfortable" | "spacious";

interface DensityContextValue {
	density: Density;
	setDensity: (d: Density) => void;
	spacing: {
		page: string;
		card: string;
		gap: string;
		text: string;
	};
}

const spacingMap: Record<Density, DensityContextValue["spacing"]> = {
	compact: {
		page: "p-3",
		card: "p-3",
		gap: "gap-2",
		text: "text-sm",
	},
	comfortable: {
		page: "p-6",
		card: "p-5",
		gap: "gap-4",
		text: "text-sm",
	},
	spacious: {
		page: "p-8",
		card: "p-6",
		gap: "gap-6",
		text: "text-base",
	},
};

const DensityContext = createContext<DensityContextValue | null>(null);

export function DensityProvider({ children }: { children: React.ReactNode }) {
	const [density, setDensity] = useState<Density>("comfortable");

	return (
		<DensityContext.Provider
			value={{ density, setDensity, spacing: spacingMap[density] }}
		>
			{children}
		</DensityContext.Provider>
	);
}

export function useDensity() {
	const ctx = useContext(DensityContext);
	if (!ctx) throw new Error("useDensity must be used within DensityProvider");
	return ctx;
}
