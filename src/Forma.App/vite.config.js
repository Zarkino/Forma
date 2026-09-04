import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig(({command,mode,ssrBuild}) => {
    const base = process.env.GITHUB_ACTIONS
        ? `/${process.env.GITHUB_REPOSITORY.split('/')[1]}/`
        : '/'

    return {
        base: base,
        plugins: [react()],
        build: {
            outDir: "../../dist",
            emptyOutDir: true
        }
    }
})