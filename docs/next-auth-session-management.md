# NextAuth.js Session Management with Azure AD in Next.js

## How It Works

- When a user signs in with Azure AD, next-auth receives an access token and user info.
- next-auth stores the session data (including user info and, if configured, the access token) in an **encrypted JWT** inside a browser cookie (e.g., `next-auth.session-token`).
- The cookie is:
  - **HTTP-only** (not accessible to JavaScript in the browser)
  - **Encrypted and signed** using your `NEXTAUTH_SECRET`
  - **Sent with every request** to your Next.js app
- **Only your Next.js app** (with the correct secret) can decrypt and read the session contents.
- The backend is **stateless**: all session info is in the cookie, so no server-side session storage is needed.
- On the server, you access the session with `getServerSession`. On the client, you can use `useSession` (if you expose the data).

## Security Notes
- The session cookie is opaque to the browser and users.
- The access token is only available server-side unless you explicitly expose it (not recommended).
- If you rotate your `NEXTAUTH_SECRET`, all sessions are invalidated.

## Summary Table

| What's in the browser? | Who can read it?         | How is it protected?         |
|------------------------|-------------------------|------------------------------|
| Encrypted JWT cookie   | Only your Next.js app   | HTTP-only, encrypted, signed |

---

**In short:**
- All session info is stored securely in the browser cookie.
- Only your Next.js app can read or use the session data.
- The backend remains stateless and secure. 