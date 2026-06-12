import {
	Select,
	SelectContent,
	SelectItem,
	SelectTrigger,
	SelectValue,
} from "@shiron/ui/components/ui/select";
import { MainStatType, getMainStatName } from "@/lib/echoStats";

const MAIN_STAT_ENTRIES = Object.values(MainStatType)
	.filter((v): v is MainStatType => typeof v === "number")
	.map((value) => ({ value, label: getMainStatName(value) }));

interface MainStatSelectProps {
	value?: MainStatType;
	onValueChange?: (value: MainStatType) => void;
	placeholder?: string;
}

export function MainStatSelect({
	value,
	onValueChange,
	placeholder = "Select main stat...",
}: MainStatSelectProps) {
	return (
		<Select
			value={value !== undefined ? String(value) : undefined}
			onValueChange={(v) => onValueChange?.(Number(v) as MainStatType)}
		>
			<SelectTrigger>
				<SelectValue placeholder={placeholder} />
			</SelectTrigger>
			<SelectContent>
				{MAIN_STAT_ENTRIES.map(({ value, label }) => (
					<SelectItem key={value} value={String(value)}>
						{label}
					</SelectItem>
				))}
			</SelectContent>
		</Select>
	);
}
