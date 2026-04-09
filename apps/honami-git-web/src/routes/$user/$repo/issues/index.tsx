import { createFileRoute } from "@tanstack/react-router";
import { IssueList } from "@/components/issues/IssueList";

export const Route = createFileRoute("/$user/$repo/issues/")({
	component: RepoIssuesPage,
});

function RepoIssuesPage() {
	return <IssueList />;
}
