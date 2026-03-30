/// <reference types="vite/client" />

interface ImportMetaEnv {
    readonly VITE_ARCHIVE_API_URL: string;
}

interface ImportMeta {
    readonly env: ImportMetaEnv;
}
