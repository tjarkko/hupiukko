#!/bin/bash

set -e

MIGRATIONS_DIR="backend/Hupiukko.DbMigrations"

echo "🗄️  Running database migrations using DbUp..."

pushd "$MIGRATIONS_DIR" > /dev/null

dotnet run

popd > /dev/null

echo "✅ Database migrations complete." 