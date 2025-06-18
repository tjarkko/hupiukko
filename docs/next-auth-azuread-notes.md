# NextAuth.js + Azure AD: Redirect URI Platform Notes

## Why the Redirect URI Must Be Set Under the Web Platform

When using Next.js with next-auth and Azure AD, the authentication flow is handled server-side via API routes (e.g., `/api/auth/callback/azure-ad`). This means:

- The OAuth2 Authorization Code flow is completed by the backend (Node.js server), not purely in the browser.
- Azure AD expects redirect URIs for server-side flows to be registered under the **Web** platform, not the **SPA** platform.
- If you register the redirect URI under the SPA platform, Azure AD will require PKCE (Proof Key for Code Exchange), which is not compatible with the way next-auth handles the flow.
- Registering the URI under the Web platform allows the classic Authorization Code flow, which is what next-auth uses.

## Summary Table

| Platform in Azure AD | Use case                                 | PKCE required? | Works with next-auth? |
|----------------------|------------------------------------------|----------------|----------------------|
| Web                  | SSR/Node.js backend (Next.js, Express)   | No             | ✅ Yes               |
| SPA                  | Pure client-side SPA (MSAL.js, etc.)     | Yes            | 🚫 Not for next-auth |

## Troubleshooting

- **Error:** `AADSTS9002325: Proof Key for Code Exchange is required for cross-origin authorization code redemption`
  - **Fix:** Move your redirect URI (e.g., `http://localhost:3000/api/auth/callback/azure-ad`) to the **Web** platform in your Azure AD app registration.

## References
- [next-auth Azure AD Provider Docs](https://next-auth.js.org/providers/azure-ad)
- [Microsoft Docs: Redirect URIs](https://learn.microsoft.com/en-us/azure/active-directory/develop/reply-url)

---

**In summary:**
- For Next.js + next-auth, always register your redirect URI under the Web platform in Azure AD.
- The SPA platform is only for pure client-side apps using MSAL.js or similar libraries. 