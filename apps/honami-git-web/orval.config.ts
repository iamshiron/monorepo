import {defineConfig} from "orval";

export default defineConfig({
    archive: {
        output: {
            mode: "tags-split",
            target: "src/api/archive.ts",
            schemas: "src/api/model",
            client: "react-query",
            mock: false,
            httpClient: "axios",
            biome: true,
            override: {
                mutator: {
                    path: "./src/lib/custom-instance.ts",
                    name: "customInstance",
                },
            },
        },
        input: {
            target: "http://127.0.0.1:2015/openapi/v1.json",
        },
    },
});
