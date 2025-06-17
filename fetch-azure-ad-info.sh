#!/bin/bash

# Fetch Azure AD app registration info for backend and frontend
# Replace these with your actual app registration display names
BACKEND_APP_NAME="hupiukko-backend"
FRONTEND_APP_NAME="hupiukko-frontend"

echo "Fetching Azure AD info..."

# Tenant ID
tenant_id=$(az account show --query tenantId -o tsv)
echo "Tenant ID: $tenant_id"

# Domain Name (primary)
domain_name=$(az ad signed-in-user show --query userPrincipalName -o tsv | awk -F'@' '{print $2}')
echo "Domain Name: $domain_name"

# Backend App Info
backend_client_id=$(az ad app list --display-name "$BACKEND_APP_NAME" --query "[0].appId" -o tsv)
backend_object_id=$(az ad app list --display-name "$BACKEND_APP_NAME" --query "[0].id" -o tsv)
backend_app_uri=$(az ad app list --display-name "$BACKEND_APP_NAME" --query "[0].identifierUris[0]" -o tsv)

echo "\nBackend App Registration:"
echo "  Client ID: $backend_client_id"
echo "  Object ID: $backend_object_id"
echo "  Application ID URI: $backend_app_uri"

# Frontend App Info
frontend_client_id=$(az ad app list --display-name "$FRONTEND_APP_NAME" --query "[0].appId" -o tsv)
frontend_object_id=$(az ad app list --display-name "$FRONTEND_APP_NAME" --query "[0].id" -o tsv)
frontend_redirect_uris=$(az ad app list --display-name "$FRONTEND_APP_NAME" --query "[0].web.redirectUris" -o tsv)

echo "\nFrontend App Registration:"
echo "  Client ID: $frontend_client_id"
echo "  Object ID: $frontend_object_id"
echo "  Redirect URIs:"
echo "$frontend_redirect_uris"

echo "\nDone." 