# Token Expiration Handling: Next.js + next-auth + .NET Backend

## Overview

When using a stateless authentication system (e.g., Azure AD, JWTs) with a Next.js frontend (next-auth) and a .NET backend, handling token expiration gracefully is crucial for both security and user experience.

---

## 1. What Happens When a Token Expires?
- The access token (JWT) issued by Azure AD (or another IdP) has a limited lifetime (e.g., 1 hour).
- When the frontend (or proxy) calls the backend API with an expired token, the backend's authentication middleware will reject the request.

---

## 2. How Should the Backend Respond?
- **Return HTTP 401 Unauthorized** for expired or invalid tokens.
- Do **not** return HTTP 500 (Internal Server Error) for authentication failures.
- In ASP.NET Core, handle token expiration and custom 401 responses in the **OnChallenge** event of `JwtBearerEvents` (not `OnAuthenticationFailed`), to avoid issues with the response already being started.

### Example: Handling Token Expiration in OnChallenge
```csharp
options.Events = new JwtBearerEvents
{
    OnAuthenticationFailed = context =>
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(context.Exception, "Authentication failed.");
        // Do NOT write to the response here!
        return Task.CompletedTask;
    },
    OnChallenge = context =>
    {
        if (!context.Response.HasStarted)
        {
            context.HandleResponse(); // Suppress the default logic
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var error = context.ErrorDescription?.ToLowerInvariant();
            if (error != null && error.Contains("expired"))
            {
                return context.Response.WriteAsync("{\"error\": \"token expired\"}");
            }
            return context.Response.WriteAsync("{\"error\": \"authentication failed\"}");
        }
        return Task.CompletedTask;
    }
};
```
- This ensures the client can distinguish between expired tokens and other errors, and avoids the "response has already started" error.

---

## 3. How Should the Frontend Handle 401 Errors?
- **Intercept 401 responses** from the backend (e.g., in your API proxy, Axios/fetch wrapper, or react-query global error handler).
- When a 401 is detected:
  - Option 1: **Sign out the user** (using `signOut()` from next-auth) and redirect to the login page.
  - Option 2: **Attempt to refresh the token** (if using refresh tokens or silent SSO), then retry the request.
- With next-auth, the most robust approach is to sign out the user and let them re-authenticate.

### Example: react-query Global Error Handler
```js
import { signOut } from "next-auth/react";
import { QueryClient } from "@tanstack/react-query";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      onError: (error) => {
        if (error?.response?.status === 401) {
          signOut();
        }
      },
    },
    mutations: {
      onError: (error) => {
        if (error?.response?.status === 401) {
          signOut();
        }
      },
    },
  },
});
```

---

## 4. Troubleshooting & Tips
- **Backend returns 500 instead of 401?**
  - Ensure you handle token expiration in `OnChallenge`, not `OnAuthenticationFailed`.
  - Only set the status code if `!context.Response.HasStarted`.
- **Frontend shows user as signed in, but backend rejects token?**
  - next-auth session cookie may still be valid, but the access token is expired. Handle 401s globally.
- **Token refresh?**
  - If you use refresh tokens, implement silent refresh in next-auth. Otherwise, sign out and require re-login.
- **Logging:**
  - Log authentication failures in the backend for easier debugging.

---

## 5. Summary Table
| Layer     | What to do on token expiration?         |
|-----------|----------------------------------------|
| Backend   | Return HTTP 401 Unauthorized (OnChallenge) |
| Frontend  | Intercept 401, sign out or refresh     |

---

**In summary:**
- Always return 401 for expired/invalid tokens in the backend (handle in OnChallenge).
- Handle 401 globally in the frontend to sign out or refresh.
- Never return 500 for authentication failures.
- Use logging and clear error messages for easier debugging. 