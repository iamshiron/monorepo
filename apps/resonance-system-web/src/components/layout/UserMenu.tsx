import { useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "@tanstack/react-router";
import { toast } from "sonner";
import { getGetMeQueryKey, useGetMe, useLogout } from "@/api/account/account";
import { Avatar, AvatarFallback } from "@shiron/ui/components/ui/avatar";
import { Button } from "@shiron/ui/components/ui/button";
import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuItem,
	DropdownMenuLabel,
	DropdownMenuSeparator,
	DropdownMenuTrigger,
} from "@shiron/ui/components/ui/dropdown-menu";
import { Spinner } from "@shiron/ui/components/ui/spinner";

function getInitials(name: string): string {
	return name
		.split(/\s+/)
		.slice(0, 2)
		.map((w) => w.charAt(0).toUpperCase())
		.join("");
}

export function UserMenu() {
	const navigate = useNavigate();
	const queryClient = useQueryClient();
	const { data: me, isLoading } = useGetMe();
	const logoutMutation = useLogout();

	const isAuthenticated = me?.status === 200 && me.data.userName;

	if (isLoading) {
		return <Spinner className="size-5" />;
	}

	if (!isAuthenticated) {
		return (
			<Button asChild variant="ghost" size="sm">
				<a href="/auth/login">Sign In</a>
			</Button>
		);
	}

	const user = me.data;
	const displayName = user.displayName ?? user.userName ?? "User";
	async function handleLogout() {
		try {
			await logoutMutation.mutateAsync();
			await queryClient.invalidateQueries({ queryKey: getGetMeQueryKey() });
			toast.success("Logged out successfully.");
			await navigate({ to: "/" });
		} catch {
			toast.error("Failed to log out. Please try again.");
		}
	}

	return (
		<DropdownMenu>
			<DropdownMenuTrigger asChild>
				<Button variant="ghost" size="icon" className="rounded-full">
					<Avatar size="sm">
						<AvatarFallback>{getInitials(displayName)}</AvatarFallback>
					</Avatar>
				</Button>
			</DropdownMenuTrigger>
			<DropdownMenuContent align="end" className="w-48">
				<DropdownMenuLabel>{displayName}</DropdownMenuLabel>
				<DropdownMenuSeparator />
				<DropdownMenuItem
					onSelect={() =>
						navigate({ to: "/$user", params: { user: `@${user.userName}` } })
					}
				>
					Profile
				</DropdownMenuItem>
				<DropdownMenuSeparator />
				<DropdownMenuItem
					variant="destructive"
					onSelect={handleLogout}
					disabled={logoutMutation.isPending}
				>
					{logoutMutation.isPending ? (
						<Spinner className="size-3.5" />
					) : (
						"Log Out"
					)}
				</DropdownMenuItem>
			</DropdownMenuContent>
		</DropdownMenu>
	);
}
