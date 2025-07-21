#!/bin/bash

set -e

ENVIRONMENT="${1:-dev}"  # Usage: ./set-sql-ad-admin.sh [dev|prod]
ENV_FILE="environments.yml"

# Extract values from environments.yml and trim whitespace
RESOURCE_GROUP=$(yq ".${ENVIRONMENT}.resource_group" "$ENV_FILE" | xargs)
SQL_SERVER_NAME=$(yq ".${ENVIRONMENT}.sql_server_name" "$ENV_FILE" | xargs)
GROUP_NAME=$(yq ".${ENVIRONMENT}.sql_admin_group_name // \"sql-admins-${ENVIRONMENT}\"" "$ENV_FILE" | xargs)

# Debug output
echo "RESOURCE_GROUP: $RESOURCE_GROUP"
echo "SQL_SERVER_NAME: $SQL_SERVER_NAME"
echo "GROUP_NAME: $GROUP_NAME"

# Get the objectId of the group and trim whitespace
GROUP_OBJECT_ID=$(az ad group show --group "$GROUP_NAME" --query id -o tsv | xargs)
echo "GROUP_OBJECT_ID: $GROUP_OBJECT_ID"

az sql server ad-admin create \
  --resource-group "$RESOURCE_GROUP" \
  --server "$SQL_SERVER_NAME" \
  --display-name "$GROUP_NAME" \
  --object-id "$GROUP_OBJECT_ID"

echo "Set Azure AD admin for SQL server '$SQL_SERVER_NAME' to group '$GROUP_NAME' in environment '$ENVIRONMENT'." 