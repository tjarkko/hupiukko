#!/bin/bash
set -e

# When running in Docker on macOS, 'localhost' inside the container does not refer to the host machine.
# We override the connection string to use 'host.docker.internal', which Docker maps to the host's IP.
# This allows the container to connect to SQL Server running on the host.
# When running directly on macOS, appsettings.Development.json uses 'localhost' as normal.
docker run --rm -it \
  -e "ConnectionStrings__DefaultConnection=Server=host.docker.internal,1433;Database=HupiukkoDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;" \
  -p 7151:443 hupiukko-api-local:latest