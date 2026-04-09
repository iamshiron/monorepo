import { createFileRoute, Outlet } from "@tanstack/react-router";
import { RepoHeader } from "@/components/repository/RepoHeader";
import { RepoTabs } from "@/components/repository/RepoTabs";

export const Route = createFileRoute("/$user/$repo")({
	component: RepoLayout,
});

function RepoLayout() {
	const { user, repo } = Route.useParams();
	const isEmpty = repo === "empty";

	return (
		<div className="p-6 space-y-4">
			<RepoHeader
				owner={user}
				repo={repo}
				visibility={isEmpty ? "private" : "public"}
			/>
			{!isEmpty && <RepoTabs owner={user} repo={repo} />}
			<Outlet />
		</div>
	);
}
