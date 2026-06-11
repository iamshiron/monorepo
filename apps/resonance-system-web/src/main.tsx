import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { RouterProvider, createRouter } from "@tanstack/react-router";
import { ThemeProvider } from "next-themes";
import "@/styles/globals.css";
import { routeTree } from "./routeTree.gen";
import { queryClient } from "./lib/query-client";

const router = createRouter({
	routeTree,
	context: { queryClient },
});

declare module "@tanstack/react-router" {
	interface Register {
		router: typeof router;
	}
}

// biome-ignore lint/style/noNonNullAssertion: standard React entry point
createRoot(document.getElementById("root")!).render(
	<StrictMode>
		<ThemeProvider
			attribute="class"
			defaultTheme="dark"
			enableSystem
			disableTransitionOnChange
		>
			<QueryClientProvider client={queryClient}>
				<RouterProvider router={router} />
				<ReactQueryDevtools buttonPosition="bottom-right" />
			</QueryClientProvider>
		</ThemeProvider>
	</StrictMode>,
);
