#!/bin/bash
set -e

cd backend/Hupiukko.Api

docker build -t hupiukko-api-local:latest -f Dockerfile.local . 