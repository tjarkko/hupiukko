#!/bin/bash

# Setup ASP.NET Core Development Certificate
echo "Setting up ASP.NET Core development certificate..."

# Check if certificate exists
if dotnet dev-certs https --check; then
    echo "✅ Development certificate already exists and is trusted"
else
    echo "🔧 Creating and trusting development certificate..."
    dotnet dev-certs https --trust
    
    if [ $? -eq 0 ]; then
        echo "✅ Development certificate created and trusted successfully"
    else
        echo "❌ Failed to create development certificate"
        echo "Try running: dotnet dev-certs https --clean && dotnet dev-certs https --trust"
        exit 1
    fi
fi

echo ""
echo "🎉 Development environment is ready!"
echo "You can now run: dotnet run" 