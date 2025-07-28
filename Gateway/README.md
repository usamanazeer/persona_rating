# Gateway Service

This is an API Gateway built with Ocelot that routes requests to various microservices.

## Configuration

The gateway is configured using `ocelot.json` which defines routing rules for different services.

### AuthService Routes

- **Base Path**: `/api/auth`
- **Target Service**: `authservice:80`
- **Endpoints**:
  - `POST /api/auth/google-login` - Google authentication
  - `POST /api/auth/refresh` - Token refresh

## Running the Gateway

### Local Development
```bash
dotnet run
```

### Docker
```bash
docker-compose up gateway
```

## Port Configuration

- **Gateway**: `http://localhost:8080`
- **AuthService**: `http://localhost:5001`

## Example Usage

```bash
# Google Login
curl -X POST http://localhost:8080/api/auth/google-login \
  -H "Content-Type: application/json" \
  -d '{"idToken": "your-google-id-token"}'

# Refresh Token
curl -X POST http://localhost:8080/api/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{"refreshToken": "your-refresh-token"}'
``` 