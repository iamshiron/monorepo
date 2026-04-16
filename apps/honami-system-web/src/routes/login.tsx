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
import { Checkbox } from "@shiron/ui/components/ui/checkbox";
import { Field, FieldLabel } from "@shiron/ui/components/ui/field";
import { Spinner } from "@shiron/ui/components/ui/spinner";
import { Separator } from "@shiron/ui/components/ui/separator";
import { useLogin } from "@/api/account/account";
import { toast } from "sonner";

export const Route = createFileRoute("/login")({
	component: LoginPage,
});

function LoginPage() {
	const navigate = useNavigate();
	const login = useLogin();

	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");
	const [rememberMe, setRememberMe] = useState(false);

	const handleSubmit = (e: React.FormEvent) => {
		e.preventDefault();
		login.mutate(
			{
				data: { email, password, rememberMe },
			},
			{
				onSuccess: () => {
					toast.success("Logged in successfully");
					navigate({ to: "/chat" });
				},
				onError: () => {
					toast.error("Invalid email or password");
				},
			},
		);
	};

	return (
		<div className="flex items-center justify-center min-h-[calc(100vh-8rem)]">
			<Card className="w-full max-w-sm">
				<CardHeader>
					<CardTitle>Welcome back</CardTitle>
					<CardDescription>Sign in to your account to continue</CardDescription>
				</CardHeader>
				<CardContent>
					<form onSubmit={handleSubmit} className="space-y-4">
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
								placeholder="Enter your password"
								value={password}
								onChange={(e) => setPassword(e.target.value)}
								required
								autoComplete="current-password"
							/>
						</Field>
						<Field orientation="horizontal">
							<Checkbox
								checked={rememberMe}
								onCheckedChange={(v) => setRememberMe(v === true)}
							/>
							<FieldLabel>Remember me</FieldLabel>
						</Field>
						<Button type="submit" className="w-full" disabled={login.isPending}>
							{login.isPending ? <Spinner className="size-4" /> : "Sign in"}
						</Button>
					</form>
				</CardContent>
				<CardFooter className="flex-col gap-3">
					<Separator />
					<p className="text-xs text-muted-foreground text-center">
						Don&apos;t have an account?{" "}
						<Link
							to="/register"
							className="text-primary underline underline-offset-4 hover:text-primary/80"
						>
							Create one
						</Link>
					</p>
				</CardFooter>
			</Card>
		</div>
	);
}
