/// <reference types="vite/client" />

interface ImportMetaEnv {
    readonly VITE_MUTILS_API_URL: string;
    readonly VITE_MUTILS_DISCORD_CLIENT_ID: string;
    readonly VITE_MUTILS_DISCORD_REDIRECT_URI: string;
}

interface ImportMeta {
    readonly env: ImportMetaEnv;
}
