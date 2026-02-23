/** @type {import('tailwindcss').Config} */
export default {
    content: [
        "./index.html",
        "./src/**/*.{js,ts,jsx,tsx}",
    ],
    theme: {
        extend: {
            colors: {
                primary: "#1A8CB0",
                accent: "#C7AE6A",
                bgLight: "#E6F3FB",
            }
        },
    },
    plugins: [],
}
