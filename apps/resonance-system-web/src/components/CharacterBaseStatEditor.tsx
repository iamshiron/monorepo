import { useMemo, useState } from "react";
import type { CharacterDTO } from "@/api/model/characterDTO";

interface CharacterBaseStatEditorProps {
	character: CharacterDTO;
}

export function CharacterBaseStatEditor({
	character,
}: CharacterBaseStatEditorProps) {
	const [forte0Level, setForte0Level] = useState(10);
	const [forte1Level, setForte1Level] = useState(10);
	const [forte2Level, setForte2Level] = useState(10);
	const [forte3Level, setForte3Level] = useState(10);
	const [forte4Level, setForte4Level] = useState(10);

	const [forte0Extra, setForte0Extra] = useState(2);
	const [forte1Extra, setForte1Extra] = useState(2);
	const [forte2Extra, setForte2Extra] = useState(2);
	const [forte3Extra, setForte3Extra] = useState(2);
	const [forte4Extra, setForte4Extra] = useState(2);

	const [characterLevel, setCharacterLevel] = useState(90);
	const [weaponLevel, setWeaponLevel] = useState(1);

	const [characterSequence, setCharacterSequence] = useState(0);
	const [weaponRank, setWeaponRank] = useState(1);

	return <div>{character.name}</div>;
}
