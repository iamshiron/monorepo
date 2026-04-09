import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/$user/profile")({
	component: RouteComponent,
});

function RouteComponent() {
	const params = Route.useParams();
	return <p>Hello {params.user}</p>;
}
