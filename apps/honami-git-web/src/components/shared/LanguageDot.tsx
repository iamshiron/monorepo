const languageColors: Record<string, string> = {
	rust: "bg-orange-500",
	typescript: "bg-blue-500",
	javascript: "bg-yellow-400",
	python: "bg-green-500",
	go: "bg-cyan-400",
	ruby: "bg-red-500",
	java: "bg-red-600",
	"c++": "bg-pink-500",
	c: "bg-gray-500",
	swift: "bg-orange-400",
	kotlin: "bg-purple-500",
	shell: "bg-emerald-400",
};

export function LanguageDot({
	language,
	className = "",
}: {
	language: string;
	className?: string;
}) {
	const color = languageColors[language.toLowerCase()] ?? "bg-muted-foreground";
	return (
		<span
			className={`inline-block size-2.5 rounded-full ${color} ${className}`}
		/>
	);
}
