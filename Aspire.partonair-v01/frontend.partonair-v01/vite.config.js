import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";

// https://vite.dev/config/
export default defineConfig({
	plugins: [react()],
	server: {
		proxy: {
			"/api": {
				target:
					process.env.services__server__https__0 ||
					process.env.services__server__http__0 ||
					"https://localhost:7540",
				changeOrigin: true,
				secure: false,
			},
		},
	},
});
