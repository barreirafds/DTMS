using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Auth0.AspNetCore.Authentication;
using System.Security.Claims;
using BusinessLogicLayer.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DTMS.Pages
{
    public class CallbackModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Get the authentication result from Auth0
                var loginResult = await HttpContext.AuthenticateAsync(Auth0Constants.AuthenticationScheme);

                if (!loginResult.Succeeded)
                {
                    throw new Exception("Authentication failed");
                }

                // Get user email from Auth0 claims
                var email = loginResult.Principal.FindFirst(ClaimTypes.Email)?.Value 
                    ?? loginResult.Principal.FindFirst("email")?.Value;

                // Get or create local user ID from database
                int userId = GetOrCreateLocalUserId(email ?? "unknown@auth0.com");

                // Create new claims identity with local user ID
                var claims = loginResult.Principal.Claims.ToList();
                
                // Remove existing NameIdentifier if any
                claims.RemoveAll(c => c.Type == ClaimTypes.NameIdentifier);
                
                // Add local user ID as NameIdentifier
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));

                // Create new principal with updated claims
                var identity = new ClaimsIdentity(claims, Auth0Constants.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Sign in the user with updated claims
                await HttpContext.SignInAsync(Auth0Constants.AuthenticationScheme, principal);

                // Redirect to home page
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                // Log the error and redirect to login with error
                return RedirectToPage("/Login", new { error = ex.Message });
            }
        }

        private int GetOrCreateLocalUserId(string email)
        {
            // Get user repository from service provider
            var userRepository = HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            
            // Extract username from email (part before @)
            var username = email.Split('@')[0];

            // Try to find existing user by username
            var users = userRepository.GetUsers();
            var existingUser = users.FirstOrDefault(u => u.user1?.Equals(username, StringComparison.OrdinalIgnoreCase) == true);

            if (existingUser?.id != null)
            {
                return existingUser.id.Value;
            }

            // User doesn't exist, create a default user
            // Note: In production, you might want to handle this differently
            userRepository.CreateUser(username, "N/A", "User");
            
            // Get the newly created user
            users = userRepository.GetUsers();
            var newUser = users.FirstOrDefault(u => u.user1?.Equals(username, StringComparison.OrdinalIgnoreCase) == true);
            
            return newUser?.id ?? 1; // Fallback to ID 1 if creation fails
        }
    }
}

