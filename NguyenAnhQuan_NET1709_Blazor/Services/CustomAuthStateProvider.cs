using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Blazored.SessionStorage;

namespace NguyenAnhQuan_NET1709_Blazor.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorage;

        public CustomAuthStateProvider(ISessionStorageService sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userRole = await _sessionStorage.GetItemAsync<int?>("AccountRole");
                
                // If no role is found, return unauthenticated user
                if (userRole == null)
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                // Try to get other user data
                var userId = await _sessionStorage.GetItemAsync<int?>("AccountId");
                var userEmail = await _sessionStorage.GetItemAsync<string>("AccountEmail");

                // Create claims identity
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Role, userRole.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, userId?.ToString() ?? "0"),
                    new Claim(ClaimTypes.Email, userEmail ?? string.Empty)
                }, "session");

                var user = new ClaimsPrincipal(identity);
                return new AuthenticationState(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAuthenticationStateAsync: {ex.Message}");
                // Return unauthenticated user on error
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}