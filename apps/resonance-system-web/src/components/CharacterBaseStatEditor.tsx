import { Input } from "@shiron/ui/components/ui/input";
import { Slider } from "@shiron/ui/components/ui/slider.tsx";
import { useState } from "react";
import type { CharacterDTO } from "@/api/model/characterDTO";
import type { EchoInstanceDTO } from "@/api/model/echoInstanceDTO";
import {
	CHARACTER_MAX_LEVEL,
	CHARACTER_MAX_SEQUENCE,
	FORTE_MAX_LEVEL,
	WEAPON_MAX_LEVEL,
	WEAPON_MAX_RANK,
} from "@/lib/constants.ts";
import { calculateTotalEchoCritValue } from "@/lib/echoStats.ts";
import { MainStatSelect } from "@/components/ui/MainStatSelect.tsx";
import { SubStatSelect } from "@/components/ui/SubStatSelect.tsx";
import { Separator } from "@shiron/ui/components/ui/separator.tsx";

interface CharacterBaseStatEditorProps {
	character: CharacterDTO;
}

interface WeaponMockDTO {
	name: string;
	weaponStats: string;
}

const WEAPON_MOCK_DATA: WeaponMockDTO = {
	name: "Kumokiri",
	weaponStats:
		"ATK is increased by 12%. When the wielder casts Intro Skill or inflicts Negative Statuses, they gain 8% Resonance Liberation DMG Bonus, stacking up to 3 times for 15s. At max stacks, when Resonators in the team inflict Negative Statuses, they gain 24% All-Attribute DMG Bonus for 15s. Effects of the same name cannot be stacked.",
};

export function CharacterBaseStatEditor({
	character,
}: CharacterBaseStatEditorProps) {
	const [forte0Level, setForte0Level] = useState(FORTE_MAX_LEVEL);
	const [forte1Level, setForte1Level] = useState(FORTE_MAX_LEVEL);
	const [forte2Level, setForte2Level] = useState(FORTE_MAX_LEVEL);
	const [forte3Level, setForte3Level] = useState(FORTE_MAX_LEVEL);
	const [forte4Level, setForte4Level] = useState(FORTE_MAX_LEVEL);

	const [forte0Extra, setForte0Extra] = useState(2);
	const [forte1Extra, setForte1Extra] = useState(2);
	const [forte2Extra, setForte2Extra] = useState(2);
	const [forte3Extra, setForte3Extra] = useState(2);
	const [forte4Extra, setForte4Extra] = useState(2);

	const fortes = [
		{
			key: 0,
			level: forte0Level,
			extra: forte0Extra,
			setLevel: setForte0Level,
			setExtra: setForte0Extra,
		},
		{
			key: 1,
			level: forte1Level,
			extra: forte1Extra,
			setLevel: setForte1Level,
			setExtra: setForte1Extra,
		},
		{
			key: 2,
			level: forte2Level,
			extra: forte2Extra,
			setLevel: setForte2Level,
			setExtra: setForte2Extra,
		},
		{
			key: 3,
			level: forte3Level,
			extra: forte3Extra,
			setLevel: setForte3Level,
			setExtra: setForte3Extra,
		},
		{
			key: 4,
			level: forte4Level,
			extra: forte4Extra,
			setLevel: setForte4Level,
			setExtra: setForte4Extra,
		},
	];

	const [characterLevel, setCharacterLevel] = useState([CHARACTER_MAX_LEVEL]);
	const [weaponLevel, setWeaponLevel] = useState([WEAPON_MAX_LEVEL]);

	const [characterSequence, setCharacterSequence] = useState(0);
	const [weaponRank, setWeaponRank] = useState(1);

	const [echoes, setEchoes] = useState<EchoInstanceDTO[]>(
		Array.from({ length: 5 }, () => ({
			index: 0,
			level: 25,
			name: "Nightmare: Roseshroom",
			cost: 1,
			mainStatType: 7,
			mainStatValue: 30,
			subStats: [
				{
					index: 0,
					type: 2,
					value: 10.1,
				},
				{
					index: 1,
					type: 4,
					value: 40,
				},
				{
					index: 2,
					type: 1,
					value: 17.4,
				},
				{
					index: 3,
					type: 5,
					value: 8.6,
				},
				{
					index: 4,
					type: 7,
					value: 9.3,
				},
			],
		})),
	);

	return (
		<div className="flex flex-col gap-4 w-full">
			<div className="grid grid-cols-2 gap-4">
				<div className="flex flex-col gap-4">
					<h2 className="text-2xl">{character.name}</h2>
					<div className="flex flex-col gap-2">
						<p className="text-nowrap">Level {characterLevel}</p>
						<Slider
							min={1}
							max={CHARACTER_MAX_LEVEL}
							step={1}
							value={characterLevel}
							onValueChange={setCharacterLevel}
						/>
					</div>
					<Input
						type="number"
						min={0}
						max={CHARACTER_MAX_SEQUENCE}
						value={characterSequence}
						onChange={(e) => setCharacterSequence(Number(e.target.value))}
					/>
				</div>
				<div className="flex flex-col gap-4">
					<h2 className="text-2xl">{WEAPON_MOCK_DATA.name}</h2>
					<div className="flex flex-col gap-2">
						<p className="text-nowrap">Level {weaponLevel}</p>
						<Slider
							min={1}
							max={WEAPON_MAX_LEVEL}
							step={1}
							value={weaponLevel}
							onValueChange={setWeaponLevel}
						/>
					</div>
					<div className="flex flex-row gap-2">
						<Input
							className="flex shrink"
							type="number"
							min={1}
							max={WEAPON_MAX_RANK}
							value={weaponRank}
							onChange={(e) => setWeaponRank(Number(e.target.value))}
						/>
						<p className="grow text-xs">{WEAPON_MOCK_DATA.weaponStats}</p>
					</div>
				</div>
			</div>

			<div className="flex flex-row gap-4 w-full">
				{fortes.map((forte) => (
					<div key={forte.key} className="flex w-full">
						<Input
							className="flex shrink"
							type="number"
							min={0}
							max={FORTE_MAX_LEVEL}
							value={forte.level}
							onChange={(e) => forte.setLevel(Number(e.target.value))}
						/>
					</div>
				))}
				number
			</div>

			<p>CV: {calculateTotalEchoCritValue(echoes)}</p>
			<div className="grid grid-cols-5 gap-2">
				{echoes
					.sort((a, b) => a.cost - b.cost)
					.map((echo, index) => (
						<div
							key={index}
							className="flex flex-col gap-2 w-full bg-card p-2 rounded-lg"
						>
							<p className="text-sm">{echo.name}</p>
							<span className="flex flex-row justify-between gap-4">
								<MainStatSelect value={echo.mainStatType} />
								<span className="flex flex-row gap-1 items-center">
									<Input
										className="text-right w-16"
										type="number"
										value={echo.mainStatValue}
										onChange={(e) => {
											console.log(e.target.value);
											const value = Number(e.target.value);
											if (value >= 0) {
												echo.mainStatValue = value;
											}
											echoes.sort((a, b) => a.cost - b.cost);
											setEchoes([...echoes]);
										}}
									/>
									<p>%</p>
								</span>
							</span>

							<Separator />

							{echo.subStats
								?.sort((a, b) => a.index - b.index)
								.map((subStat) => (
									<div
										key={subStat.index}
										className="flex flex-row justify-between gap-2"
									>
										<SubStatSelect value={subStat.type} />
										<span className="flex flex-row">
											<Input
												className="text-right w-16"
												type="number"
												value={subStat.value}
											/>
										</span>
									</div>
								))}
						</div>
					))}
			</div>
		</div>
	);
}
