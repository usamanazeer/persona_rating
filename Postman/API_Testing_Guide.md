# API Testing Guide

This guide explains how to test the Persona Rating API using the provided Postman collection.

## Prerequisites

1. **Postman** installed on your machine
2. **Gateway** service running on `http://localhost:5132`
3. **AuthService** running on `http://localhost:5001`

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
- Expected: Should return a response (may be 404 if health endpoint not implemented, but should not be connection refused)

**Test Direct AuthService:**
- Request: `GET http://localhost:5001/api/auth` (or any endpoint)
- Expected: Should return a response (may be 404 but should not be connection refused)

### Step 2: Test Authentication Endpoints

#### 2.1 Google Login (Through Gateway)
- **Request**: `POST http://localhost:5132/api/auth/google-login`
- **Headers**: `Content-Type: application/json`
- **Body**:
```json
{
  "idToken": "your-google-id-token-here"
}
```

**Expected Response** (Success):
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh-token-here",
  "expiresIn": 3600,
  "user": {
    "id": "user-id",
    "email": "user@example.com",
    "name": "User Name"
  }
}
```

**Expected Response** (Error):
```json
{
  "message": "Invalid Google ID token"
}
```

#### 2.2 Refresh Token (Through Gateway)
- **Request**: `POST http://localhost:5132/api/auth/refresh`
- **Headers**: `Content-Type: application/json`
- **Body**:
```json
{
  "refreshToken": "your-refresh-token-here"
}
```

#### 2.3 Get Current User (Through Gateway)
- **Request**: `GET http://localhost:5132/api/auth/me`
- **Headers**: `Authorization: Bearer {{access_token}}`

### Step 3: Test Direct AuthService (Bypass Gateway)

Use the "Direct AuthService" requests to test the AuthService directly, bypassing the Gateway. This helps isolate whether issues are with the Gateway routing or the AuthService itself.

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

**If you get `ECONNREFUSED` on port 5001:**
1. Check if AuthService is running:
   ```bash
   dotnet run --project AuthService/Api
   ```
2. Verify no other process is using port 5001:
   ```bash
   netstat -an | findstr :5001
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

## Environment Variables

The Postman environment automatically manages these variables:

- `{{base_url}}` - Gateway base URL (http://localhost:5132)
- `{{auth_service_url}}` - AuthService base URL (http://localhost:5001)
- `{{access_token}}` - JWT access token (auto-populated after successful login)
- `{{refresh_token}}` - Refresh token (auto-populated after successful login)
- `{{user_id}}` - User ID (auto-populated after successful login)
- `{{email}}` - User email (auto-populated after successful login)

## Test Data

For testing purposes, you can use these sample tokens (replace with real ones):

### Google ID Token (for testing)
```json
{
  "idToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjEyMzQ1Njc4OTAiLCJ0eXAiOiJKV1QifQ..."
}
```

### Refresh Token (for testing)
```json
{
  "refreshToken": "refresh-token-sample-12345"
}
```

## Running Tests in Sequence

1. **Start with Gateway Health Check** - Verify Gateway is accessible
2. **Test Direct AuthService** - Verify AuthService is accessible
3. **Test Google Login through Gateway** - Verify routing works
4. **Test Refresh Token** - Verify token refresh functionality
5. **Test Get Current User** - Verify JWT authentication

## Expected Status Codes

- `200` - Success
- `400` - Bad Request (invalid input)
- `401` - Unauthorized (invalid/missing token)
- `404` - Not Found (endpoint doesn't exist)
- `500` - Internal Server Error

## Performance Testing

The collection includes automatic tests that verify:
- Response time is under 2000ms
- Status code is 200 (for successful requests)
- Tokens are automatically extracted and stored

## Next Steps

Once basic functionality is working:
1. Add more comprehensive test cases
2. Test error scenarios
3. Test with real Google ID tokens
4. Add load testing for performance validation 