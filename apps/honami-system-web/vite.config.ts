import {defineConfig} from "vite";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";
import {TanStackRouterVite} from "@tanstack/router-vite-plugin";
import path from "path";

export default defineConfig({
    envDir: "../../",
    plugins: [TanStackRouterVite(), tailwindcss(), react()],
    resolve: {
        alias: {
            "@": path.resolve(__dirname, "./src"),
            "@shiron/ui": path.resolve(__dirname, "../../packages/ui/src"),
        },
    },
    server: {
        port: 1913,
        proxy: {
            "/api": {
                target: "http://localhost:2013",
                changeOrigin: true,
            },
        },
    },
});
