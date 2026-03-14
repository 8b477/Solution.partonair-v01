import flowbitePlugin from "flowbite/plugin";
/** @type {import('tailwindcss').Config} */
export default {
	content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
	theme: {
		extend: {
			colors: {
				electric: {
					300: "#c4b5fd",
					400: "#a78bfa",
					500: "#8b5cf6",
					600: "#7c3aed",
				},
			},
			animation: {
				float: "float 6s ease-in-out infinite",
				"pulse-glow": "pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite",
			},
			keyframes: {
				float: {
					"0%, 100%": { transform: "translate3d(0,0,0)" },
					"50%": { transform: "translate3d(0,-20px,0)" },
				},
			},
		},
	},
	plugins: [flowbitePlugin],
};
