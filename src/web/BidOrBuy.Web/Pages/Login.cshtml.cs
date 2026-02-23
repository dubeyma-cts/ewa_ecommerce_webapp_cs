using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EWA.Web.Pages;

public class LoginModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        // Already logged in — go to home
        if (HttpContext.Session.GetString("FullName") is not null)
            return RedirectToPage("/Home");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var client = _httpClientFactory.CreateClient("IdentityAPI");

        HttpResponseMessage response;
        try
        {
            response = await client.PostAsJsonAsync("/api/auth/login", new
            {
                Username,
                Password
            });
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Cannot connect to the Identity service. Please try again later.";
            return Page();
        }

        if (!response.IsSuccessStatusCode)
        {
            ErrorMessage = "Invalid username or password.";
            return Page();
        }

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        if (result is null)
        {
            ErrorMessage = "Unexpected error during login.";
            return Page();
        }

        HttpContext.Session.SetString("Token",    result.Token);
        HttpContext.Session.SetString("Username", result.Username);
        HttpContext.Session.SetString("FullName", result.FullName);
        HttpContext.Session.SetString("Role",     result.Role);
        HttpContext.Session.SetString("Email",    result.Email);

        return RedirectToPage("/Home");
    }

    private record LoginResponse(string Token, string Username, string FullName, string Role, string Email);
}
