using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ReenbitTest.UI;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ReenbitTest.UI.Interfaces;
using ReenbitTest.UI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddMudServices();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Set the base address for API calls
string apiBaseUrl = builder.Configuration["ApiEndpoints:BaseUrl"] ?? "https://reenbittestapi-ajfja4efd9bfe4eg.canadacentral-01.azurewebsites.net/api";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// Add Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Register Authentication Services
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

// Register Application Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChatHubService, ChatHubService>();

builder.Services.AddScoped<NavbarEventService>();

// Configure application settings
if (builder.Configuration["ApiEndpoints:BaseUrl"] == null)
{
    builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["ApiEndpoints:BaseUrl"] = "https://reenbittestapi-ajfja4efd9bfe4eg.canadacentral-01.azurewebsites.net/api",
        ["ApiEndpoints:ChatHub"] = "https://reenbittestapi-ajfja4efd9bfe4eg.canadacentral-01.azurewebsites.net/chathub"
    });
}

// Build and run the application
await builder.Build().RunAsync();