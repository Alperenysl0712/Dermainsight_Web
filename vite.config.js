import { resolve } from 'path';

export default {
    root: './wwwroot',
    build: {
        outDir: '../wwwroot/dist',
        emptyOutDir: true,
        rollupOptions: {
            input: {
                main: resolve(__dirname, 'wwwroot/js/main.js')
            },
            output: {
                entryFileNames: 'main.js' // Dosya adı sabit kalır
            }
        }
    }
};
