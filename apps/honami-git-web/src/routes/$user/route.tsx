import { createFileRoute, Outlet } from "@tanstack/react-router";

export const Route = createFileRoute("/$user")({
	component: RouteComponent,
});

function RouteComponent() {
	return (
		<div>
			<h1>App Layout</h1>
			<Outlet />
		</div>
	);
}
