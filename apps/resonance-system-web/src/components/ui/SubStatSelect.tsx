import {
	Select,
	SelectContent,
	SelectItem,
	SelectTrigger,
	SelectValue,
} from "@shiron/ui/components/ui/select";
import { SubStatType, getSubStatName } from "@/lib/echoStats";

const SUB_STAT_ENTRIES = Object.values(SubStatType)
	.filter((v): v is SubStatType => typeof v === "number")
	.map((value) => ({ value, label: getSubStatName(value) }));

interface SubStatSelectProps {
	value?: SubStatType;
	onValueChange?: (value: SubStatType) => void;
	placeholder?: string;
}

export function SubStatSelect({
	value,
	onValueChange,
	placeholder = "Select sub stat...",
}: SubStatSelectProps) {
	return (
		<Select
			value={value !== undefined ? String(value) : undefined}
			onValueChange={(v) => onValueChange?.(Number(v) as SubStatType)}
		>
			<SelectTrigger>
				<SelectValue placeholder={placeholder} />
			</SelectTrigger>
			<SelectContent>
				{SUB_STAT_ENTRIES.map(({ value, label }) => (
					<SelectItem key={value} value={String(value)}>
						{label}
					</SelectItem>
				))}
			</SelectContent>
		</Select>
	);
}
