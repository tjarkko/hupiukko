#!/bin/bash

set -e

CONTAINER_NAME=hupiukko-sqlserver
IMAGE_NAME=hupiukko-sqlserver

# Build the image if it doesn't exist
echo "🔍 Checking for Docker image $IMAGE_NAME..."
if [[ "$(docker images -q $IMAGE_NAME 2> /dev/null)" == "" ]]; then
  echo "📦 Building Docker image $IMAGE_NAME..."
  docker build -f Dockerfile.sqlserver -t $IMAGE_NAME .
else
  echo "✅ Docker image $IMAGE_NAME already exists."
fi

# Check if the container is already running
if [[ $(docker ps -q -f name=$CONTAINER_NAME) ]]; then
  echo "✅ SQL Server container '$CONTAINER_NAME' is already running."
  exit 0
fi

# If container exists but is stopped, start it
echo "🔍 Checking for existing container..."
if [[ $(docker ps -a -q -f name=$CONTAINER_NAME) ]]; then
  echo "▶️  Starting existing container $CONTAINER_NAME..."
  docker start $CONTAINER_NAME
else
  echo "🚀 Running new SQL Server container with Finnish_Swedish_CI_AS collation..."
  docker run -d \
    --name $CONTAINER_NAME \
    -e ACCEPT_EULA=Y \
    -e SA_PASSWORD=YourStrong!Passw0rd \
    -e MSSQL_PID=Developer \
    -e MSSQL_COLLATION=Finnish_Swedish_CI_AS \
    -p 1433:1433 \
    -v sqlserver_data:/var/opt/mssql \
    $IMAGE_NAME
fi

echo "✅ SQL Server is up and running with Finnish_Swedish_CI_AS collation!" 