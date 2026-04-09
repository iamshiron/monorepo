import { createFileRoute } from "@tanstack/react-router";
import { IssueDetail } from "@/components/issues/IssueDetail";

export const Route = createFileRoute("/$user/$repo/issues/$issueId")({
	component: IssueDetailPage,
});

function IssueDetailPage() {
	return <IssueDetail />;
}
