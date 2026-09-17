# AppTTHH

Talento Humano bounded-context service integrated with PortalCorporativo.

## Architecture

- .NET 10
- Clean Architecture: Domain, Application, Infrastructure, API
- SQL Server persistence isolated in `AppTTHHDb`
- JWT bearer authentication aligned with PortalCorporativo
- Docker-first local runtime

## API

- `GET /health` - liveness
- `GET /health/ready` - database readiness
- `GET /api/hr/employees` - authenticated employee list
- `GET /api/hr/employees/{id}` - authenticated employee lookup
- `POST /api/hr/employees` - authenticated employee creation

## Validation

```powershell
dotnet build AppTTHH.slnx -c Release
dotnet test AppTTHH.slnx -c Release --no-build
```

Portal exposes this service through `/api/hr/**`; the API container should remain internal to the shared Docker network.

## Authorization

Portal permission claims are enforced by the API:

- `hr.employees.view` for employee reads.
- `hr.employees.manage` for employee creation and future write operations.

## Required runtime configuration

The service intentionally has no fallback database password or JWT signing secret in source control. Runtime must provide:

- `ConnectionStrings__HrDb`
- `Jwt__Secret`
- `Jwt__Issuer` (defaults to `portal-corporativo` when omitted)
- `Jwt__Audience` (defaults to `portal-corporativo-clients` when omitted)
