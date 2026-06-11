import { useState, type FormEvent } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { toast } from "sonner";
import { getGetMeQueryKey, useLogin } from "@/api/account/account";
import { Button } from "@shiron/ui/components/ui/button";
import {
	Card,
	CardContent,
	CardDescription,
	CardFooter,
	CardHeader,
	CardTitle,
} from "@shiron/ui/components/ui/card";
import { Checkbox } from "@shiron/ui/components/ui/checkbox";
import {
	Field,
	FieldError,
	FieldGroup,
	FieldLabel,
} from "@shiron/ui/components/ui/field";
import { Input } from "@shiron/ui/components/ui/input";
import { Spinner } from "@shiron/ui/components/ui/spinner";

type FormErrors = {
	email?: string;
	password?: string;
};

export const Route = createFileRoute("/auth/login")({
	component: LoginPage,
});

function LoginPage() {
	const navigate = useNavigate();
	const queryClient = useQueryClient();
	const loginMutation = useLogin();

	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");
	const [rememberMe, setRememberMe] = useState(false);
	const [errors, setErrors] = useState<FormErrors>({});

	function validate(): FormErrors {
		const result: FormErrors = {};

		if (!email.trim()) {
			result.email = "Email is required.";
		} else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
			result.email = "Enter a valid email address.";
		}

		if (!password) {
			result.password = "Password is required.";
		}

		return result;
	}

	async function handleSubmit(e: FormEvent) {
		e.preventDefault();

		const validationErrors = validate();
		setErrors(validationErrors);

		if (validationErrors.email || validationErrors.password) {
			return;
		}

		try {
			const result = await loginMutation.mutateAsync({
				data: { email, password, rememberMe },
			});

			if (result.status === 200) {
				await queryClient.invalidateQueries({ queryKey: getGetMeQueryKey() });
				toast.success("Logged in successfully.");
				await navigate({ to: "/dashboard" });
			} else {
				toast.error("Invalid email or password.");
			}
		} catch {
			toast.error("Something went wrong. Please try again.");
		}
	}

	const isPending = loginMutation.isPending;

	return (
		<div className="flex items-center justify-center min-h-[60vh]">
			<Card className="mx-auto w-full max-w-sm">
				<CardHeader>
					<CardTitle>Sign In</CardTitle>
					<CardDescription>Enter your credentials to continue.</CardDescription>
				</CardHeader>
				<form onSubmit={handleSubmit} noValidate>
					<CardContent>
						<FieldGroup>
							<Field data-invalid={!!errors.email || undefined}>
								<FieldLabel htmlFor="login-email">Email</FieldLabel>
								<Input
									id="login-email"
									type="email"
									placeholder="you@example.com"
									autoComplete="email"
									value={email}
									onChange={(e) => {
										setEmail(e.target.value);
										if (errors.email) {
											setErrors((prev) => ({
												...prev,
												email: undefined,
											}));
										}
									}}
									disabled={isPending}
									aria-invalid={!!errors.email}
								/>
								{errors.email && <FieldError>{errors.email}</FieldError>}
							</Field>

							<Field data-invalid={!!errors.password || undefined}>
								<FieldLabel htmlFor="login-password">Password</FieldLabel>
								<Input
									id="login-password"
									type="password"
									autoComplete="current-password"
									value={password}
									onChange={(e) => {
										setPassword(e.target.value);
										if (errors.password) {
											setErrors((prev) => ({
												...prev,
												password: undefined,
											}));
										}
									}}
									disabled={isPending}
									aria-invalid={!!errors.password}
								/>
								{errors.password && <FieldError>{errors.password}</FieldError>}
							</Field>

							<Field orientation="horizontal">
								<Checkbox
									id="login-remember"
									checked={rememberMe}
									onCheckedChange={(checked) => setRememberMe(checked === true)}
									disabled={isPending}
								/>
								<FieldLabel htmlFor="login-remember">Remember me</FieldLabel>
							</Field>
						</FieldGroup>
					</CardContent>
					<CardFooter>
						<Button type="submit" className="w-full" disabled={isPending}>
							{isPending ? <Spinner className="size-4" /> : "Sign In"}
						</Button>
					</CardFooter>
				</form>
			</Card>
		</div>
	);
}
