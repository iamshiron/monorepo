import {defineConfig} from 'orval';

export default defineConfig({
    honamiGit: {
        output: {
            mode: 'tags-split',
            target: 'src/api/honamiGit.ts',
            schemas: 'src/api/model',
            client: 'react-query',
            httpClient: 'axios',
            biome: true,
        },
        input: {
            target: 'http://127.0.0.1:2014/openapi/v1.json',
        },
    },
});
