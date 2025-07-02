/** @type {import('next').NextConfig} */
const nextConfig = {
  // Enable static optimization where possible
  reactStrictMode: true,
  
  // Ensure environment variables are available at build time
  env: {
    // Public variables that are safe to expose to the browser
    NEXT_PUBLIC_API_URL: process.env.NEXT_PUBLIC_API_URL,
  },
  
  // Configure server-side environment variables
  serverRuntimeConfig: {
    // Will only be available on the server side
    NEXTAUTH_SECRET: process.env.NEXTAUTH_SECRET,
    NEXTAUTH_URL: process.env.NEXTAUTH_URL,
  },

  output: 'standalone',
}

module.exports = nextConfig 