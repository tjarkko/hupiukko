# Azure AD Token Troubleshooting: v1/v2 Endpoints and Audience Issues

## Problem 1: v1 vs v2 Endpoint (Issuer Mismatch)

- **Symptom:**
  - Backend logs show: `SecurityTokenInvalidIssuerException: IDX10205: Issuer validation failed. Issuer: 'https://sts.windows.net/<tenant-id>/'... Did not match: ... 'https://login.microsoftonline.com/<tenant-id>/v2.0'`
- **Cause:**
  - Azure AD can issue tokens from either the v1.0 (`sts.windows.net`) or v2.0 (`login.microsoftonline.com/<tenant-id>/v2.0`) endpoint.
  - Backend expects v2.0 tokens, but frontend or app registration is using v1.0.
- **Solution:**
  - Ensure both frontend and backend use v2.0 endpoints.
  - In Azure AD app registration manifest, set:
    ```json
    "accessTokenAcceptedVersion": 2
    ```
  - In frontend (next-auth, MSAL, etc.), ensure the authorization URL is `/oauth2/v2.0/authorize`.
  - In backend, authority should be:
    ```csharp
    options.Authority = "https://login.microsoftonline.com/<tenant-id>/v2.0";
    ```

---

## Problem 2: Audience Mismatch (`aud` claim)

- **Symptom:**
  - Backend logs show: `SecurityTokenInvalidAudienceException: IDX10214: Audience validation failed. Audiences: '<client-id>'. Did not match: 'api://<client-id>'...`
- **Cause:**
  - Azure AD can issue tokens with either the Application ID URI (`api://<client-id>`) or the plain client ID as the audience.
  - This can depend on how the Application ID URI is set in Azure, how the scope is requested, and sometimes Azure AD quirks/caching.
- **Solution:**
  - In Azure Portal, under "Expose an API", ensure the Application ID URI is set (e.g., `api://<client-id>`).
  - In frontend, request the scope as `api://<client-id>/user_impersonation` (or your custom scope).
  - In backend, accept both as valid audiences:
    ```csharp
    options.TokenValidationParameters.ValidAudiences = new[]
    {
        "api://<client-id>",
        "<client-id>"
    };
    ```
  - Add `ClientId` to your AzureAd config if needed.

---

## General Tips

- Always check the actual `aud` and `iss` claims in your JWT at [jwt.ms](https://jwt.ms/).
- If you change the Application ID URI, re-consent may be required and Azure AD may cache old values for a while.
- Use logging in your backend's JWT Bearer events to debug authentication failures.
- Use `ClaimTypes.NameIdentifier` or the full URI claim type to get the Azure AD object ID (`oid`).

---

**Summary:**
- Use v2.0 endpoints everywhere.
- Align Application ID URI and client ID between Azure, frontend, and backend.
- Accept both possible audiences in backend if needed.
- Use logging and [jwt.ms](https://jwt.ms/) to debug claims and tokens. 