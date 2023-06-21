import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { viteSingleFile } from 'vite-plugin-singlefile'

export default defineConfig(({command,mode,ssrBuild}) => {
    if (mode === 'single-file') {
		return {
	    	plugins: [react(), viteSingleFile()],
	    	root: "./src",
	    	build: {
				outDir: "../dist-single-file",
	    	}
		}
    } else {
		return {
		    plugins: [react()],
		    root: "./src",
		    build: {
				outDir: "../dist",
		    }
		}
    }
})