#!/bin/bash
set -e

# Run migrations using DbUp with --drop-db to drop and recreate the database
echo "Running migrations with DbUp (dropping DB first)..."
cd backend/Hupiukko.DbMigrations
dotnet run -- --drop-db

echo "Database rebuild complete!" 