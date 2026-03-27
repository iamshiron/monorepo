import { defineConfig } from 'orval';

export default defineConfig({
    mutils: {
        output: {
            mode: 'tags-split',
            target: 'src/api/mutils.ts',
            schemas: 'src/api/model',
            client: 'react-query',
            mock: true,
            httpClient: 'axios',
            biome: true
        },
        input: {
            target: 'http://127.0.0.1:1810/openapi/v1.json',
        },
    },
});
