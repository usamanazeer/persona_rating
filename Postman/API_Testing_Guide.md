# API Testing Guide

This guide explains how to test the Persona Rating API using the provided Postman collection.

## Prerequisites

1. **Postman** installed on your machine
2. **Gateway** service running on `http://localhost:5132`
3. **AuthService** running on `http://localhost:5175`

## Setup Instructions

### 1. Import Postman Collection

1. Open Postman
2. Click **Import** button
3. Import the following files:
   - `Persona_Rating_API.postman_collection.json` - Main collection
   - `Persona_Rating_Environment.postman_environment.json` - Environment variables

### 2. Select Environment

1. In the top-right corner of Postman, select **"Persona Rating Environment"**
2. This will enable the environment variables for all requests

## Testing Flow

### Step 1: Verify Services are Running

**Test Gateway Health Check:**
- Request: `GET http://localhost:5132/health`
- Expected: Should return a JSON response with status "healthy"

**Test Direct AuthService:**
- Request: `GET http://localhost:5175/api/auth` (or any endpoint)
- Expected: Should return a response (may be 404 but should not be connection refused)

### Step 2: Test Authentication Endpoints

#### 2.1 Google Login (Verified Email) - Should Succeed
- **Request**: `POST http://localhost:5132/api/auth/google-login`
- **Headers**: `Content-Type: application/json`
- **Body**:
```json
{
  "idToken": "verified-token"
}
```

**Expected Response** (Success - 200):
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh-token-here",
  "userId": "user-id",
  "email": "john.doe@example.com",
  "name": "John Doe",
  "picture": "https://lh3.googleusercontent.com/a-/AOh14GgMockProfilePic"
}
```

#### 2.2 Google Login (Unverified Email) - Should Fail
- **Request**: `POST http://localhost:5132/api/auth/google-login`
- **Headers**: `Content-Type: application/json`
- **Body**:
```json
{
  "idToken": "unverified-token"
}
```

**Expected Response** (Error - 401):
```json
{
  "message": "Email address is not verified. Please verify your email address with Google before signing in."
}
```

#### 2.3 Google Login (Invalid Token) - Should Fail
- **Request**: `POST http://localhost:5132/api/auth/google-login`
- **Headers**: `Content-Type: application/json`
- **Body**:
```json
{
  "idToken": "invalid-token"
}
```

**Expected Response** (Error - 401):
```json
{
  "message": "Invalid Google token"
}
```

#### 2.4 Refresh Token (Through Gateway)
- **Request**: `POST http://localhost:5132/api/auth/refresh`
- **Headers**: `Content-Type: application/json`
- **Body**:
```json
{
  "refreshToken": "your-refresh-token-here"
}
```

#### 2.5 Get Current User (Through Gateway)
- **Request**: `GET http://localhost:5132/api/auth/me`
- **Headers**: `Authorization: Bearer {{access_token}}`

### Step 3: Test Direct AuthService (Bypass Gateway)

Use the "Direct AuthService" requests to test the AuthService directly, bypassing the Gateway. This helps isolate whether issues are with the Gateway routing or the AuthService itself.

## Email Verification Feature

The API now includes email verification as a security requirement:

### ✅ **Verified Email Users**
- Can successfully authenticate
- Receive access and refresh tokens
- Can access protected endpoints

### ❌ **Unverified Email Users**
- Cannot authenticate
- Receive 401 Unauthorized error
- Must verify email with Google before signing in

### 🔧 **Test Tokens**
For development and testing purposes, the following special tokens are available:

- **`verified-token`** - Simulates a user with verified email (should succeed)
- **`unverified-token`** - Simulates a user with unverified email (should fail)
- **`invalid-token`** - Simulates an invalid token (should fail)

## Troubleshooting

### Connection Refused Errors

**If you get `ECONNREFUSED` on port 5132:**
1. Check if Gateway is running:
   ```bash
   dotnet run --project Gateway
   ```
2. Verify no other process is using port 5132:
   ```bash
   netstat -an | findstr :5132
   ```

**If you get `ECONNREFUSED` on port 5175:**
1. Check if AuthService is running:
   ```bash
   dotnet run --project AuthService/Api
   ```
2. Verify no other process is using port 5175:
   ```bash
   netstat -an | findstr :5175
   ```

### Gateway Not Routing Properly

1. **Check Ocelot Configuration**: Verify `Gateway/ocelot.json` is correct
2. **Check Docker Services**: If using Docker, ensure all services are running:
   ```bash
   docker-compose ps
   ```
3. **Check Gateway Logs**: Look for routing errors in Gateway console output

### Authentication Issues

1. **Invalid Token**: Ensure you're using a valid Google ID token
2. **Token Format**: Verify the request body matches the expected format
3. **CORS Issues**: Check if CORS is properly configured in both Gateway and AuthService
4. **Email Verification**: Ensure the user's email is verified with Google

## Environment Variables

The Postman environment automatically manages these variables:

- `{{base_url}}` - Gateway base URL (http://localhost:5132)
- `{{auth_service_url}}` - AuthService base URL (http://localhost:5175)
- `{{access_token}}` - JWT access token (auto-populated after successful login)
- `{{refresh_token}}` - Refresh token (auto-populated after successful login)
- `{{user_id}}` - User ID (auto-populated after successful login)
- `{{email}}` - User email (auto-populated after successful login)

## Test Data

For testing purposes, you can use these special tokens:

### Verified Email (Success)
```json
{
  "idToken": "verified-token"
}
```

### Unverified Email (Failure)
```json
{
  "idToken": "unverified-token"
}
```

### Invalid Token (Failure)
```json
{
  "idToken": "invalid-token"
}
```

## Running Tests in Sequence

1. **Start with Gateway Health Check** - Verify Gateway is accessible
2. **Test Direct AuthService** - Verify AuthService is accessible
3. **Test Verified Email Login** - Should succeed and return tokens
4. **Test Unverified Email Login** - Should fail with 401 error
5. **Test Invalid Token Login** - Should fail with 401 error
6. **Test Refresh Token** - Verify token refresh functionality
7. **Test Get Current User** - Verify JWT authentication

## Expected Status Codes

- `200` - Success (verified email login, health check)
- `400` - Bad Request (invalid input)
- `401` - Unauthorized (invalid token, unverified email)
- `404` - Not Found (endpoint doesn't exist)
- `500` - Internal Server Error

## Performance Testing

The collection includes automatic tests that verify:
- Response time is under 2000ms
- Tokens are automatically extracted and stored

## Next Steps

Once basic functionality is working:
1. Add more comprehensive test cases
2. Test error scenarios
3. Test with real Google ID tokens
4. Add load testing for performance validation 