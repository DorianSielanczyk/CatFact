# CatFact API 🐱

## 🚀 Tech Stack

*   **Framework:** .NET 8.0 (ASP.NET Core Web API)
*   **Language:** C# 12
*   **Resilience:** `Microsoft.Extensions.Http.Resilience` (Polly v8)
*   **Testing:** `xUnit`, `Moq`
*   **Documentation:** Swagger / OpenAPI

## 📡 Endpoints

### `POST /api/FactResponse`
Fetches a random cat fact from the external API, appends it (along with its length) to a local text file located in the `Responses/` directory, and returns the fetched fact.
*   **Rate Limit:** 5 requests per 10 seconds.
*   **File Limit:** Will throw a handled exception if the local file exceeds 5MB.

## 🛠️ Getting Started

1. Clone the repository.
2. Ensure you have the .NET 8 SDK installed.
3. Run the application:
   ```bash
   dotnet build
   dotnet run
