#!/bin/bash

CONTAINER_NAME=hupiukko-sqlserver

if [[ $(docker ps -q -f name=$CONTAINER_NAME) ]]; then
  echo "🛑 Stopping SQL Server container $CONTAINER_NAME..."
  docker stop $CONTAINER_NAME
else
  echo "ℹ️  SQL Server container $CONTAINER_NAME is not running."
fi

if [[ $(docker ps -a -q -f name=$CONTAINER_NAME) ]]; then
  echo "🗑️  Removing SQL Server container $CONTAINER_NAME..."
  docker rm $CONTAINER_NAME
else
  echo "ℹ️  SQL Server container $CONTAINER_NAME does not exist."
fi

echo "✅ SQL Server container cleanup complete." 