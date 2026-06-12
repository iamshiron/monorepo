import type { EchoDTO, EchoInstanceDTO, EchoSubStatDTO } from "@/api/model";

export enum MainStatType {
	AttackPercent,
	DefencsPercent,
	HPPercent,
	GlacioDMGPercent,
	SpectroDMGPercent,
	AeroDMGPercent,
	ElectroDMGPercent,
	HavocDMGPercent,
	FusionDMGPercent,
	EnergyRegen,
	CritRate,
	CritDMG,
	HealingBonus,
}

export enum SubStatType {
	Attack,
	Defense,
	HP,
	AttackPercent,
	DefensePercent,
	HPPercent,
	CritRate,
	CritDMG,
	EnergyRegen,
	BasicAttackDMG,
	HeavyAttackDMG,
	ResonanceSkillDMG,
	ResonanceLiberationDMG,
}

export function getMainStatName(stat: MainStatType) {
	switch (stat) {
		case MainStatType.AttackPercent:
			return "ATK%";
		case MainStatType.DefencsPercent:
			return "DEF%";
		case MainStatType.HPPercent:
			return "HP%";
		case MainStatType.GlacioDMGPercent:
			return "Glacio DMG Bonus";
		case MainStatType.SpectroDMGPercent:
			return "Spectro DMG Bonus";
		case MainStatType.AeroDMGPercent:
			return "Aero DMG Bonus";
		case MainStatType.ElectroDMGPercent:
			return "Electro DMG Bonus";
		case MainStatType.HavocDMGPercent:
			return "Havoc DMG Bonus";
		case MainStatType.FusionDMGPercent:
			return "Fusion DMG Bonus";
		case MainStatType.EnergyRegen:
			return "Energy Regen";
		case MainStatType.CritRate:
			return "Crit. Rate";
		case MainStatType.CritDMG:
			return "Crit. DMG";
		case MainStatType.HealingBonus:
			return "Healing Bonus";
	}
}
export function getSubStatName(stat: SubStatType) {
	switch (stat) {
		case SubStatType.Attack:
			return "ATK";
		case SubStatType.Defense:
			return "DEF";
		case SubStatType.HP:
			return "HP";
		case SubStatType.AttackPercent:
			return "ATK%";
		case SubStatType.DefensePercent:
			return "DEF%";
		case SubStatType.HPPercent:
			return "HP%";
		case SubStatType.CritRate:
			return "Crit. Rate";
		case SubStatType.CritDMG:
			return "Crit. DMG";
		case SubStatType.EnergyRegen:
			return "Energy Regen";
		case SubStatType.BasicAttackDMG:
			return "Basic Attack DMG Bonus";
		case SubStatType.HeavyAttackDMG:
			return "Heavy Attack DMG Bonus";
		case SubStatType.ResonanceSkillDMG:
			return "Resonance Skill DMG Bonus";
		case SubStatType.ResonanceLiberationDMG:
			return "Resonance Liberation DMG Bonus";
	}
}

export function isSubStatPercent(stat: SubStatType) {
	switch (stat) {
		case SubStatType.AttackPercent:
		case SubStatType.DefensePercent:
		case SubStatType.HPPercent:
		case SubStatType.CritRate:
		case SubStatType.CritDMG:
		case SubStatType.EnergyRegen:
		case SubStatType.BasicAttackDMG:
		case SubStatType.HeavyAttackDMG:
		case SubStatType.ResonanceSkillDMG:
		case SubStatType.ResonanceLiberationDMG:
			return true;
		case SubStatType.Attack:
		case SubStatType.Defense:
		case SubStatType.HP:
			return false;
	}
}

export function critValue(critDMG: number | null, critRate: number | null) {
	return (
		(critDMG == null ? 0 : critDMG) + (critRate == null ? 0 : critRate * 2)
	);
}

export function calculateTotalEchoCritValue(echoes: EchoInstanceDTO[]) {
	var critValue = 0;

	for (const echo of echoes) {
		critValue += calculateEchoCritValue(echo);
	}

	return critValue;
}

/**
 * Calculate the crit value of the echo
 * Formula: CritValue = CritRate * 2 + CritDMG
 * @param echo The echo to calculate the crit value of
 */
export function calculateEchoCritValue(echo: EchoInstanceDTO) {
	let critValue = 0;

	if ((echo.mainStatType as MainStatType) === MainStatType.CritRate)
		critValue += (echo.mainStatValue as number) * 2;
	if ((echo.mainStatType as MainStatType) === MainStatType.CritDMG)
		critValue += echo.mainStatValue as number;

	if (echo.subStats === undefined) return critValue;
	for (const stat of echo.subStats) {
		if (stat.type === SubStatType.CritRate)
			critValue += (stat.value as number) * 2;
		if (stat.type === SubStatType.CritDMG) critValue += stat.value as number;
	}

	return critValue;
}
