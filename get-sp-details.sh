#!/bin/bash

# Get Service Principal Details for SQL Bootstrap
# Run this script to get the details needed for manual SQL setup

echo "🔍 Retrieving service principal details..."

# Get service principal details
SP_OBJECT_ID=$(az ad sp list --display-name "github-actions-hupiukko" --query "[0].id" -o tsv)
SP_APP_ID=$(az ad sp list --display-name "github-actions-hupiukko" --query "[0].appId" -o tsv)
SP_DISPLAY_NAME=$(az ad sp list --display-name "github-actions-hupiukko" --query "[0].displayName" -o tsv)

echo ""
echo "📋 Service Principal Details:"
echo "Display Name: $SP_DISPLAY_NAME"
echo "Object ID: $SP_OBJECT_ID"
echo "App ID: $SP_APP_ID"
echo ""

echo "🔧 Manual Setup Commands:"
echo ""
echo "# 1. Set Azure AD admin for SQL Server (run this first):"
echo "az sql server ad-admin create \\"
echo "  --resource-group hupiukko-dev-rg \\"
echo "  --server-name hupiukko-sql-dev \\"
echo "  --display-name \"$SP_DISPLAY_NAME\" \\"
echo "  --object-id \"$SP_OBJECT_ID\""
echo ""
echo "# 2. SQL script to create users (run after setting admin):"
echo "-- For migration service principal"
echo "CREATE USER [$SP_DISPLAY_NAME] FROM EXTERNAL PROVIDER;"
echo "ALTER ROLE db_ddladmin ADD MEMBER [$SP_DISPLAY_NAME];"
echo "ALTER ROLE db_datareader ADD MEMBER [$SP_DISPLAY_NAME];"
echo "ALTER ROLE db_datawriter ADD MEMBER [$SP_DISPLAY_NAME];"
echo ""
echo "-- For backend managed identity (get this from Azure portal or Bicep output)"
echo "CREATE USER [hupiukko-backend-identity] FROM EXTERNAL PROVIDER;"
echo "ALTER ROLE db_datareader ADD MEMBER [hupiukko-backend-identity];"
echo "ALTER ROLE db_datawriter ADD MEMBER [hupiukko-backend-identity];" 