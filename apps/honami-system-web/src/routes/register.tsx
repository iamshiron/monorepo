import { useState } from "react";
import { createFileRoute, Link, useNavigate } from "@tanstack/react-router";
import {
	Card,
	CardHeader,
	CardTitle,
	CardDescription,
	CardContent,
	CardFooter,
} from "@shiron/ui/components/ui/card";
import { Button } from "@shiron/ui/components/ui/button";
import { Input } from "@shiron/ui/components/ui/input";
import { Field, FieldLabel } from "@shiron/ui/components/ui/field";
import { Spinner } from "@shiron/ui/components/ui/spinner";
import { Separator } from "@shiron/ui/components/ui/separator";
import { useRegister } from "@/api/account/account";
import { toast } from "sonner";

export const Route = createFileRoute("/register")({
	component: RegisterPage,
});

function RegisterPage() {
	const navigate = useNavigate();
	const register = useRegister();

	const [displayName, setDisplayName] = useState("");
	const [userName, setUserName] = useState("");
	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");

	const handleSubmit = (e: React.FormEvent) => {
		e.preventDefault();
		register.mutate(
			{
				data: { displayName, userName, email, password },
			},
			{
				onSuccess: () => {
					toast.success("Account created. Please sign in.");
					navigate({ to: "/login" });
				},
				onError: () => {
					toast.error("Registration failed. Check your details.");
				},
			},
		);
	};

	return (
		<div className="flex items-center justify-center min-h-[calc(100vh-8rem)]">
			<Card className="w-full max-w-sm">
				<CardHeader>
					<CardTitle>Create an account</CardTitle>
					<CardDescription>Get started with HonamiSystem</CardDescription>
				</CardHeader>
				<CardContent>
					<form onSubmit={handleSubmit} className="space-y-4">
						<Field>
							<FieldLabel>Display name</FieldLabel>
							<Input
								placeholder="Your name"
								value={displayName}
								onChange={(e) => setDisplayName(e.target.value)}
								required
								maxLength={32}
								autoComplete="name"
							/>
						</Field>
						<Field>
							<FieldLabel>Username</FieldLabel>
							<Input
								placeholder="username"
								value={userName}
								onChange={(e) => setUserName(e.target.value)}
								required
								maxLength={32}
								autoComplete="username"
							/>
						</Field>
						<Field>
							<FieldLabel>Email</FieldLabel>
							<Input
								type="email"
								placeholder="you@example.com"
								value={email}
								onChange={(e) => setEmail(e.target.value)}
								required
								autoComplete="email"
							/>
						</Field>
						<Field>
							<FieldLabel>Password</FieldLabel>
							<Input
								type="password"
								placeholder="At least 4 characters"
								value={password}
								onChange={(e) => setPassword(e.target.value)}
								required
								minLength={4}
								autoComplete="new-password"
							/>
						</Field>
						<Button
							type="submit"
							className="w-full"
							disabled={register.isPending}
						>
							{register.isPending ? (
								<Spinner className="size-4" />
							) : (
								"Create account"
							)}
						</Button>
					</form>
				</CardContent>
				<CardFooter className="flex-col gap-3">
					<Separator />
					<p className="text-xs text-muted-foreground text-center">
						Already have an account?{" "}
						<Link
							to="/login"
							className="text-primary underline underline-offset-4 hover:text-primary/80"
						>
							Sign in
						</Link>
					</p>
				</CardFooter>
			</Card>
		</div>
	);
}
