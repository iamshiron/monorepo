import { useMemo } from "react";

const WEEKS = 52;
const DAYS = 7;

interface Cell {
	id: string;
	level: number;
}

interface Week {
	id: string;
	cells: Cell[];
}

function generateContributions(): Week[] {
	return Array.from({ length: WEEKS }, (_, w) => ({
		id: `w${w}`,
		cells: Array.from({ length: DAYS }, (_, d) => {
			const rand = Math.random();
			const level =
				rand < 0.3
					? 0
					: rand < 0.5
						? 1
						: rand < 0.7
							? 2
							: rand < 0.85
								? 3
								: rand < 0.95
									? 4
									: 5;
			return { id: `w${w}-d${d}`, level };
		}),
	}));
}

const intensityClasses = [
	"bg-muted/50",
	"bg-primary/20",
	"bg-primary/35",
	"bg-primary/50",
	"bg-primary/70",
	"bg-primary",
];

const legendItems = intensityClasses.map((cls, i) => ({ id: `leg-${i}`, cls }));

export function ContributionMap() {
	const weeks = useMemo(() => generateContributions(), []);

	return (
		<div>
			<h3 className="text-sm font-semibold mb-4">Contributions</h3>
			<div className="glass rounded-xl border border-border/50 p-4 overflow-x-auto">
				<div className="flex gap-[3px] min-w-fit">
					{weeks.map((week) => (
						<div key={week.id} className="flex flex-col gap-[3px]">
							{week.cells.map((cell) => (
								<div
									key={cell.id}
									className={`size-[10px] rounded-[2px] transition-colors ${intensityClasses[cell.level]}`}
								/>
							))}
						</div>
					))}
				</div>
				<div className="flex items-center gap-1.5 mt-3 text-[10px] text-muted-foreground">
					<span>Less</span>
					{legendItems.map((item) => (
						<div
							key={item.id}
							className={`size-[10px] rounded-[2px] ${item.cls}`}
						/>
					))}
					<span>More</span>
				</div>
			</div>
		</div>
	);
}
