# UI Migration Plan - Frontend Service Layer

## Overview
This document outlines the plan to migrate the TournamentApp.Web frontend from directly using `HttpClient` in Razor pages to using a service layer pattern that matches the `rah-portal` project approach. This migration is necessary because the API now returns `ValidatedResponse` objects instead of direct data types.

## Current State Analysis

### Current Issues
1. **Direct HttpClient Usage**: Pages directly inject `HttpClient` and make API calls
2. **Incorrect Deserialization**: Pages expect `List<PlayerDto>` or `List<TournamentDto>` directly, but API now returns `GetPlayersResponse { Data = IEnumerable<PlayerDto> }` and `GetTournamentListResponse { Data = IEnumerable<TournamentDto> }`
3. **No Error Handling**: Current code doesn't handle `ValidatedResponse.IsFailure`, `ValidationErrors`, or `ErrorMessage` properties
4. **Inconsistent Error Messages**: Error handling doesn't properly display validation errors from the API

### Current Code Example (Players.razor)
```csharp
var response = await Http.GetAsync("api/players");
if (response.IsSuccessStatusCode)
{
    _players = await response.Content.ReadFromJsonAsync<List<PlayerDto>>();
}
```

**Problem**: This will fail because the API now returns:
```json
{
  "isSuccess": true,
  "data": [...],
  "validationErrors": [],
  "errorMessage": ""
}
```

## Portal Project Pattern

### Architecture
1. **Service Classes**: Feature-based services (e.g., `PlayerService`, `TournamentService`) that wrap HttpClient calls
2. **Extension Method**: `GetResponseData<T>()` extension method on `HttpResponseMessage` for consistent deserialization
3. **Response Types**: Frontend response types that match backend `ValidatedResponse` structure
4. **Dependency Injection**: Services registered in `Program.cs` and injected into pages

### Key Components

#### 1. Response Types (Portal Pattern)
```csharp
public class Response
{
    public bool IsSuccess { get; set; }
    public bool IsFailure => !IsSuccess;
    public string ErrorMessage { get; set; } = string.Empty;
    public IList<ValidationFailure> ValidationErrors { get; set; } = new List<ValidationFailure>();
}

public class DataResponse<T> : Response
{
    public T Data { get; set; } = default!;
}

public class CreateResponse : Response
{
    public Guid NewId { get; set; }
}
```

#### 2. Extension Method (Portal Pattern)
```csharp
public static async Task<T> GetResponseData<T>(this HttpResponseMessage? response) where T : new()
{
    if (response == null)
    {
        return new T();
    }
    
    var data = await response.Content.ReadFromJsonAsync<T>();
    return data ?? new T();
}
```

#### 3. Service Class Example (Portal Pattern)
```csharp
public class PlayerService : IPlayerService
{
    private readonly HttpClient _httpClient;
    
    public PlayerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<DataResponse<IEnumerable<PlayerDto>>> GetPlayers()
    {
        var httpResponse = await _httpClient.GetAsync("api/players");
        return await httpResponse.GetResponseData<DataResponse<IEnumerable<PlayerDto>>>();
    }
    
    public async Task<CreateResponse> AddPlayer(AddPlayerCommand command)
    {
        var httpResponse = await _httpClient.PostAsJsonAsync("api/players", command);
        return await httpResponse.GetResponseData<CreateResponse>();
    }
}
```

#### 4. Page Usage (Portal Pattern)
```csharp
@inject IPlayerService PlayerService
@inject ISnackbar Snackbar

private async Task LoadPlayers()
{
    var response = await PlayerService.GetPlayers();
    
    if (response.IsFailure)
    {
        // Handle errors
        if (response.ValidationErrors.Any())
        {
            foreach (var error in response.ValidationErrors)
            {
                Snackbar.Add(error.ErrorMessage, Severity.Error);
            }
        }
        else if (!string.IsNullOrEmpty(response.ErrorMessage))
        {
            Snackbar.Add(response.ErrorMessage, Severity.Error);
        }
        _players = new List<PlayerDto>();
        return;
    }
    
    _players = response.Data?.ToList() ?? new List<PlayerDto>();
}
```

## Migration Plan

### Phase 1: Create Infrastructure

#### 1.1 Create Response Types
- **Location**: `src/TournamentApp.Web/Responses/`
- **Files**:
  - `Response.cs` - Base response class
  - `DataResponse.cs` - Response with generic data
  - `CreateResponse.cs` - Response for create operations (includes NewId)

#### 1.2 Create Extension Method
- **Location**: `src/TournamentApp.Web/Extensions/`
- **File**: `HttpResponseMessageExtensions.cs`
- **Purpose**: Consistent deserialization of HTTP responses

#### 1.3 Update Package References
- Ensure `FluentValidation` package is referenced (for `ValidationFailure` type)

### Phase 2: Create Service Layer

#### 2.1 Create Service Interfaces
- **Location**: `src/TournamentApp.Web/Contracts/Services/`
- **Files**:
  - `IPlayerService.cs`
  - `ITournamentService.cs`
  - `IBracketService.cs`
  - `IMatchService.cs`

#### 2.2 Create Service Implementations
- **Location**: `src/TournamentApp.Web/Services/`
- **Files**:
  - `PlayerService.cs`
  - `TournamentService.cs`
  - `BracketService.cs`
  - `MatchService.cs`

**Service Responsibilities**:
- Make HTTP calls to API endpoints
- Use `GetResponseData<T>()` to deserialize responses
- Return strongly-typed response objects

### Phase 3: Register Services in DI

#### 3.1 Update Program.cs
- Register all services as scoped services
- Ensure HttpClient is configured correctly (already done)

### Phase 4: Migrate Pages

#### 4.1 Migrate Players.razor
- Replace `@inject HttpClient Http` with `@inject IPlayerService PlayerService`
- Update `LoadPlayers()` to use service and handle `DataResponse<IEnumerable<PlayerDto>>`
- Update error handling to check `IsFailure` and display `ValidationErrors`

#### 4.2 Migrate Tournaments.razor
- Replace `@inject HttpClient Http` with `@inject ITournamentService TournamentService`
- Update all methods:
  - `LoadTournaments()` - Handle `DataResponse<IEnumerable<TournamentDto>>`
  - `GenerateBracket()` - Handle `Response` or specific response type
  - Future: `CreateTournament()`, `AddPlayerToTournament()`, etc.

#### 4.3 Future Pages
- Any new pages or components that call APIs should use services

### Phase 5: Error Handling Enhancement

#### 5.1 Create Helper Method
- Optionally create extension method or helper class for displaying validation errors
- Format: Display all validation errors in snackbar or alert component

#### 5.2 Standardize Error Display
- All pages should handle errors consistently:
  1. Check `response.IsFailure`
  2. If `ValidationErrors.Any()`, display each error
  3. If `ErrorMessage` is set, display it
  4. Set UI state appropriately (empty lists, error messages, etc.)

## File Structure

```
src/TournamentApp.Web/
├── Contracts/
│   └── Services/
│       ├── IPlayerService.cs
│       ├── ITournamentService.cs
│       ├── IBracketService.cs
│       └── IMatchService.cs
├── Services/
│   ├── PlayerService.cs
│   ├── TournamentService.cs
│   ├── BracketService.cs
│   └── MatchService.cs
├── Responses/
│   ├── Response.cs
│   ├── DataResponse.cs
│   └── CreateResponse.cs
├── Extensions/
│   └── HttpResponseMessageExtensions.cs
└── Pages/
    ├── Players.razor (updated)
    └── Tournaments.razor (updated)
```

## API Response Mapping

### Current API Responses → Service Return Types

| API Endpoint | API Returns | Service Method Returns |
|-------------|-------------|----------------------|
| `GET /api/players` | `GetPlayersResponse { Data: IEnumerable<PlayerDto> }` | `DataResponse<IEnumerable<PlayerDto>>` |
| `POST /api/players` | `AddPlayerResponse { NewId: Guid }` | `CreateResponse { NewId: Guid }` |
| `GET /api/tournaments` | `GetTournamentListResponse { Data: IEnumerable<TournamentDto> }` | `DataResponse<IEnumerable<TournamentDto>>` |
| `POST /api/tournaments` | `CreateTournamentResponse { NewId: Guid }` | `CreateResponse { NewId: Guid }` |
| `GET /api/tournaments/{id}` | `GetTournamentResponse { Data: TournamentDto }` | `DataResponse<TournamentDto>` |
| `POST /api/tournaments/{id}/players` | `AddPlayerToTournamentResponse` | `Response` |
| `POST /api/tournaments/{id}/bracket/generate` | `GenerateBracketResponse` | `Response` |
| `GET /api/tournaments/{id}/bracket` | `GetBracketResponse { Data: BracketDto }` | `DataResponse<BracketDto>` |
| `POST /api/matches/{id}/score` | `UpdateMatchScoreResponse` | `Response` |

## Implementation Notes

### Response Type Conversion
The frontend `Response`, `DataResponse<T>`, and `CreateResponse` classes should match the backend structure but are separate classes in the Web project. This allows:
- Independent versioning
- Frontend-specific properties if needed
- No direct dependency on Application layer

### ValidationFailure Type
- The `ValidationFailure` type comes from FluentValidation
- Both frontend and backend use this type for validation errors
- Frontend will need to reference `FluentValidation` NuGet package or create a compatible type

### Error Handling Strategy
1. **Network Errors**: Caught as exceptions, handled separately
2. **API Errors (400 BadRequest)**: Deserialized as response with `IsFailure = true`
3. **Validation Errors**: Displayed from `ValidationErrors` collection
4. **Business Logic Errors**: Displayed from `ErrorMessage` property

## Testing Considerations

### Unit Tests
- Services can be easily unit tested with mocked HttpClient
- Response deserialization can be tested
- Error handling logic can be tested

### Integration Tests
- Pages can be tested with mocked services
- UI behavior with different response scenarios can be tested

## Migration Checklist

- [ ] Phase 1: Create Response types and Extension method
- [ ] Phase 2: Create Service interfaces and implementations
- [ ] Phase 3: Register services in DI
- [ ] Phase 4: Migrate Players.razor
- [ ] Phase 4: Migrate Tournaments.razor
- [ ] Phase 5: Enhance error handling
- [ ] Test all pages with new service layer
- [ ] Remove unused HttpClient injections from pages
- [ ] Update any other components that use HttpClient directly

## Questions for Review

1. **ValidationFailure Type**: Should we:
   - Reference FluentValidation in the Web project?
   - Create a simplified `ValidationFailure` class in the Web project?
   - Create a shared project for common types? => if adding the reference to web project is okay to do so lets do that if not simplified class

2. **Error Display**: Should we:
   - Display all validation errors at once?
   - Display errors inline in forms (when forms are added)?
   - Use a toast/snackbar for all errors? => lets create a validation component that will list all errors

3. **Service Organization**: Should services be:
   - One service per feature (PlayerService, TournamentService)?
   - One service per entity?
   - Follow the portal project's exact structure? => feature per service 

4. **Response Mapping**: Should we:
   - Map backend responses directly (matching property names)?
   - Use AutoMapper for response mapping?
   - Manually map in service methods? => lets use viewmodels with same prop names

## Next Steps

1. Review this plan
2. Answer questions above
3. Proceed with implementation phase by phase
4. Test each phase before moving to the next





