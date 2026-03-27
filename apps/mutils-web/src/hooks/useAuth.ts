import { useState, useEffect, useCallback } from "react";
import {
	getApiAuthCallback,
	postApiAuthLogout,
} from "@/api/authentication/authentication";
import type { User } from "@/types";

export function useAuth() {
	const [user, setUser] = useState<User | null>(null);
	const [isLoading, setIsLoading] = useState(true);

	useEffect(() => {
		const storedUser = localStorage.getItem("user");
		if (storedUser) {
			setUser(JSON.parse(storedUser));
		}
		setIsLoading(false);
	}, []);

	const login = useCallback(async (code: string) => {
		const redirectUri = `${window.location.origin}/auth/callback`;
		const response = await getApiAuthCallback({
			code,
			redirect_uri: redirectUri,
		});
		console.log("Auth response:", response);
		localStorage.setItem("accessToken", response.accessToken);
		localStorage.setItem("refreshToken", response.refreshToken);
		localStorage.setItem("user", JSON.stringify(response.user));
		setUser(response.user as User);
		return response.user as User;
	}, []);

	const logout = useCallback(async () => {
		try {
			await postApiAuthLogout();
		} catch {
			// Ignore logout errors
		} finally {
			localStorage.removeItem("accessToken");
			localStorage.removeItem("refreshToken");
			localStorage.removeItem("user");
			setUser(null);
			window.location.href = "/";
		}
	}, []);

	return {
		user,
		isLoading,
		isAuthenticated: !!user,
		login,
		logout,
	};
}
