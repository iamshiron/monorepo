import {defineConfig} from 'orval';

export default defineConfig({
    "resonance-system": {
        output: {
            mode: 'tags-split',
            target: 'src/api/resonance-system.ts',
            schemas: 'src/api/model',
            client: 'react-query',
            mock: false,
            httpClient: 'fetch',
            biome: true
        },
        input: {
            target: 'http://127.0.0.1:1813/openapi/v1.json',
        },
    },
});
