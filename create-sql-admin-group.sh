#!/bin/bash

set -e

ENVIRONMENT="${1:-dev}"  # Usage: ./create-sql-admin-group.sh [dev|prod]
ENV_FILE="environments.yml"

# Extract group name from environments.yml or set default
GROUP_NAME=$(yq ".${ENVIRONMENT}.sql_admin_group_name // \"sql-admins-${ENVIRONMENT}\"" "$ENV_FILE")
GROUP_DESCRIPTION="Azure SQL Server Administrators for $ENVIRONMENT"
GROUP_MAIL_NICKNAME="sqladmins${ENVIRONMENT}$(date +%s)" # must be unique

az ad group create \
  --display-name "$GROUP_NAME" \
  --mail-nickname "$GROUP_MAIL_NICKNAME" \
  --description "$GROUP_DESCRIPTION"

echo "Azure AD group '$GROUP_NAME' created for environment '$ENVIRONMENT'." 