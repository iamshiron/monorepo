import {
	createContext,
	useCallback,
	useContext,
	useEffect,
	useMemo,
	useState,
	type ReactNode,
} from "react";
import {
	getApiAuthCallback,
	postApiAuthLogout,
} from "@/api/authentication/authentication";
import type { User } from "@/types";

interface AuthContextValue {
	user: User | null;
	isLoading: boolean;
	isAuthenticated: boolean;
	login: (code: string) => Promise<User>;
	logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
	const [user, setUser] = useState<User | null>(null);
	const [isLoading, setIsLoading] = useState(true);

	useEffect(() => {
		const storedUser = localStorage.getItem("user");
		if (storedUser) {
			try {
				const parsed = JSON.parse(storedUser);
				if (parsed && typeof parsed === "object" && parsed.id) {
					setUser(parsed);
				} else {
					throw new Error("Invalid user data in localStorage");
				}
			} catch {
				localStorage.removeItem("user");
				localStorage.removeItem("accessToken");
				localStorage.removeItem("refreshToken");
			}
		}
		setIsLoading(false);
	}, []);

	const login = useCallback(async (code: string) => {
		const redirectUri = `${window.location.origin}/auth/callback`;
		const response = await getApiAuthCallback({
			code,
			redirect_uri: redirectUri,
		});
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

	const value = useMemo<AuthContextValue>(
		() => ({ user, isLoading, isAuthenticated: !!user, login, logout }),
		[user, isLoading, login, logout],
	);

	return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
	const ctx = useContext(AuthContext);
	if (!ctx) {
		throw new Error("useAuth must be used within an AuthProvider");
	}
	return ctx;
}
