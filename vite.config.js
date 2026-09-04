import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { viteSingleFile } from 'vite-plugin-singlefile'

export default defineConfig(({command,mode,ssrBuild}) => {
    const base = process.env.GITHUB_ACTIONS
        ? `/${process.env.GITHUB_REPOSITORY.split('/')[1]}/`
        : '/'

    if (mode === 'single-file') {
		return {
            base: base,
	    	plugins: [react(), viteSingleFile()],
	    	root: "./src",
	    	build: {
				outDir: "../dist-single-file",
	    	}
		}
    } else {
		return {
            base: base,
		    plugins: [react()],
		    root: "./src",
		    build: {
				outDir: "../dist",
		    }
		}
    }
})