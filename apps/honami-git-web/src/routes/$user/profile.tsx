import { createFileRoute, redirect } from "@tanstack/react-router";

export const Route = createFileRoute("/$user/profile")({
	beforeLoad: ({ params }) => {
		throw redirect({ to: "/$user", params: { user: params.user } });
	},
});
