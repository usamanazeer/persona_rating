# Postman Testing Files

This folder contains all the Postman files needed to test the Persona Rating API.

## Files Description

### 📁 **Persona_Rating_API.postman_collection.json**
- **Main Postman Collection** containing all API test requests
- Includes requests for both Gateway and direct AuthService testing
- Contains pre-request scripts and test scripts for automatic token management
- **NEW**: Email verification test scenarios (verified vs unverified emails)

### 📁 **Persona_Rating_Environment.postman_environment.json**
- **Postman Environment** with variables for different environments
- Automatically manages tokens and URLs
- Supports switching between development, staging, and production

### 📁 **API_Testing_Guide.md**
- **Comprehensive testing guide** with step-by-step instructions
- Troubleshooting section for common issues
- Expected responses and status codes
- Performance testing guidelines
- **NEW**: Email verification feature documentation

## Quick Start

1. **Import Collection**: Import `Persona_Rating_API.postman_collection.json` into Postman
2. **Import Environment**: Import `Persona_Rating_Environment.postman_environment.json` into Postman
3. **Select Environment**: Choose "Persona Rating Environment" in the top-right corner
4. **Start Testing**: Begin with "Gateway Health Check" request

## Email Verification Feature

The API now requires email verification for authentication:

### ✅ **Test Scenarios Available:**

1. **Verified Email Login** - Should succeed (200)
   - Use token: `"verified-token"`
   - Returns access and refresh tokens

2. **Unverified Email Login** - Should fail (401)
   - Use token: `"unverified-token"`
   - Returns error message about email verification

3. **Invalid Token Login** - Should fail (401)
   - Use token: `"invalid-token"`
   - Returns error message about invalid token

### 🔧 **Test Tokens:**
- `verified-token` - Simulates verified email user
- `unverified-token` - Simulates unverified email user  
- `invalid-token` - Simulates invalid token

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
- `base_url` - Gateway URL (http://localhost:5132)
- `auth_service_url` - AuthService URL (http://localhost:5175)
- `access_token` - JWT access token (auto-populated)
- `refresh_token` - Refresh token (auto-populated)
- `user_id` - User ID (auto-populated)
- `email` - User email (auto-populated)

## Testing Sequence

1. **Gateway Health Check** - Verify Gateway is running
2. **Verified Email Login** - Test successful authentication
3. **Unverified Email Login** - Test email verification requirement
4. **Invalid Token Login** - Test error handling
5. **Refresh Token** - Test token refresh functionality
6. **Get Current User** - Test JWT authentication 