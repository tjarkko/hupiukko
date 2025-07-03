# Running the Backend Locally with Docker

This guide explains how to run the Hupiukko backend API locally in a Docker container, and how to connect it to a SQL Server instance running on your Mac.

## Prerequisites

- Docker Desktop installed and running on your Mac
- SQL Server running locally on your Mac (e.g., via Docker or native install)
- The backend Docker image built (see project scripts for building instructions)

## Why Special Handling is Needed for SQL Server Connections

When running the backend directly on your Mac, the connection string in `appsettings.Development.json` uses `localhost` to connect to SQL Server:

```
Server=localhost,1433;Database=HupiukkoDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;
```

However, when running inside a Docker container, `localhost` refers to the container itself, **not your Mac**. To allow the container to connect to SQL Server running on your Mac, Docker provides a special DNS name:

- `host.docker.internal`

This resolves to your Mac's IP address from inside the container.

## How the Connection String is Handled

- When running directly on your Mac, the backend uses the connection string from `appsettings.Development.json` (with `localhost`).
- When running in Docker, the `run-backend.sh` script overrides the connection string using an environment variable, replacing `localhost` with `host.docker.internal`.

## Step-by-Step: Running the Backend in Docker

1. **Ensure SQL Server is running on your Mac**

   - If using Docker for SQL Server, make sure the container is started and port 1433 is published (e.g., `-p 1433:1433`).

2. **Build the backend Docker image**

   - Use the provided build script or run:
     ```sh
     ./build-backend-local.sh
     ```

3. **Run the backend container**

   - Use the provided script:
     ```sh
     ./run-backend.sh
     ```
   - This script sets the connection string environment variable so the backend can connect to SQL Server on your Mac.

4. **Access the API**
   - The backend API will be available at: [https://localhost:7151](https://localhost:7151)

## Troubleshooting

- **Cannot connect to SQL Server**

  - Make sure SQL Server is running and listening on port 1433.
  - Ensure your firewall allows connections to port 1433.
  - If you see errors about network or instance-specific issues, double-check that `host.docker.internal` resolves inside the container:
    ```sh
    docker run --rm busybox nslookup host.docker.internal
    ```
  - If using Apple Silicon (M1/M2), ensure your SQL Server image supports ARM64 or use an x86_64 image with emulation.

- **Culture or locale errors**
  - The Dockerfile is configured to install Finnish and US English locales. If you see `CultureNotFoundException`, rebuild the image.

## Why Not Use 'localhost' in Docker?

- In Docker, `localhost` refers to the container itself, not your Mac. Use `host.docker.internal` to reach services running on your Mac from inside the container.

---

For more details, see the comments in `run-backend.sh` and the Dockerfile.
