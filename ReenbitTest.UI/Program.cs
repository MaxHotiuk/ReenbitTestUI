using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ReenbitTest.UI;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ReenbitTest.UI.Interfaces;
using ReenbitTest.UI.Services;

// -------------------------------------------------------------------------
// Application Builder Configuration
// -------------------------------------------------------------------------
var builder = WebAssemblyHostBuilder.CreateDefault(args);

// -------------------------------------------------------------------------
// UI Component Libraries
// -------------------------------------------------------------------------
// Register MudBlazor services for modern UI components and design system
builder.Services.AddMudServices();

// -------------------------------------------------------------------------
// Root Component Registration
// -------------------------------------------------------------------------
// Register the main App component as the root of the application
builder.RootComponents.Add<App>("#app");
// Register HeadOutlet to enable dynamic modification of head elements
builder.RootComponents.Add<HeadOutlet>("head::after");

// -------------------------------------------------------------------------
// API Communication Configuration
// -------------------------------------------------------------------------
// Set the base address for API calls
string apiBaseUrl = builder.HostEnvironment.IsDevelopment() 
    ? (builder.Configuration["ApiEndpoints:BaseUrl"] ?? "http://localhost:5120/api")
    : (builder.Configuration["ApiEndpoints:BaseUrl"] ?? "https://reenbittestapi-ajfja4efd9bfe4eg.canadacentral-01.azurewebsites.net/api");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// -------------------------------------------------------------------------
// Client-side Storage
// -------------------------------------------------------------------------
// Add Blazored LocalStorage for persistent client-side data storage
// Used for storing authentication tokens and user preferences
builder.Services.AddBlazoredLocalStorage();

// -------------------------------------------------------------------------
// Authentication Configuration
// -------------------------------------------------------------------------
// Register authentication state provider to manage user authentication state
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
// Enable authorization capabilities for component-level access control
builder.Services.AddAuthorizationCore();

// -------------------------------------------------------------------------
// Application Services
// -------------------------------------------------------------------------
// Register core application services with dependency injection container
// Authentication service for user login, registration, and token management
builder.Services.AddScoped<IAuthService, AuthService>();
// Chat service for managing message communication with the API
builder.Services.AddScoped<IChatService, ChatService>();
// User service for profile management and user operations
builder.Services.AddScoped<IUserService, UserService>();
// SignalR hub service for real-time communication
builder.Services.AddScoped<IChatHubService, ChatHubService>();

// UI state management service for navbar events
builder.Services.AddScoped<NavbarEventService>();

// -------------------------------------------------------------------------
// Configuration Fallback
// -------------------------------------------------------------------------
// Configure application settings with fallback values when not provided
// This enables the application to function in both development and production
// Check if we're in development environment
var isDevelopment = builder.HostEnvironment.IsDevelopment();

if (builder.Configuration["ApiEndpoints:BaseUrl"] == null)
{
    var endpoints = new Dictionary<string, string?>();
    
    if (isDevelopment)
    {
        // Development endpoints
        endpoints["ApiEndpoints:BaseUrl"] = "http://localhost:5120/api";
        endpoints["ApiEndpoints:ChatHub"] = "http://localhost:5120/chathub";
    }
    else
    {
        // Production endpoints
        endpoints["ApiEndpoints:BaseUrl"] = "https://reenbittestapi-ajfja4efd9bfe4eg.canadacentral-01.azurewebsites.net/api";
        endpoints["ApiEndpoints:ChatHub"] = "https://reenbittestapi-ajfja4efd9bfe4eg.canadacentral-01.azurewebsites.net/chathub";
    }
    
    builder.Configuration.AddInMemoryCollection(endpoints);
}

// -------------------------------------------------------------------------
// Application Startup
// -------------------------------------------------------------------------
// Build and run the Blazor WebAssembly application
await builder.Build().RunAsync();