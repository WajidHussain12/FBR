/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './Views/**/*.cshtml',
    './Areas/**/*.cshtml',
    './wwwroot/js/**/*.js',
  ],
  theme: {
    extend: {
      colors: {
        fbr: {
          blue:  '#1d4ed8',
          green: '#16a34a',
        }
      }
    }
  },
  plugins: [],
}
