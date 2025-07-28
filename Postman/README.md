# Postman Testing Files

This folder contains all the Postman files needed to test the Persona Rating API.

## Files Description

### 📁 **Persona_Rating_API.postman_collection.json**
- **Main Postman Collection** containing all API test requests
- Includes requests for both Gateway and direct AuthService testing
- Contains pre-request scripts and test scripts for automatic token management

### 📁 **Persona_Rating_Environment.postman_environment.json**
- **Postman Environment** with variables for different environments
- Automatically manages tokens and URLs
- Supports switching between development, staging, and production

### 📁 **API_Testing_Guide.md**
- **Comprehensive testing guide** with step-by-step instructions
- Troubleshooting section for common issues
- Expected responses and status codes
- Performance testing guidelines

## Quick Start

1. **Import Collection**: Import `Persona_Rating_API.postman_collection.json` into Postman
2. **Import Environment**: Import `Persona_Rating_Environment.postman_environment.json` into Postman
3. **Select Environment**: Choose "Persona Rating Environment" in the top-right corner
4. **Start Testing**: Begin with "Gateway Health Check" request

## Folder Structure

```
Postman/
├── README.md                                    # This file
├── Persona_Rating_API.postman_collection.json   # Main collection
├── Persona_Rating_Environment.postman_environment.json # Environment variables
└── API_Testing_Guide.md                        # Testing guide
```

## Usage

See `API_Testing_Guide.md` for detailed instructions on how to use these files to test your API endpoints.

## Environment Variables

The environment file includes these variables:
- `base_url` - Gateway URL (http://localhost:8080)
- `auth_service_url` - AuthService URL (http://localhost:5001)
- `access_token` - JWT access token (auto-populated)
- `refresh_token` - Refresh token (auto-populated)
- `user_id` - User ID (auto-populated)
- `email` - User email (auto-populated) 