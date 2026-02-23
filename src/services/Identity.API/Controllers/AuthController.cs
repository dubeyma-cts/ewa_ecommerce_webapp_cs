using Microsoft.AspNetCore.Mvc;

namespace EWA.Identity.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    // Hardcoded demo users — POC only, not for production
    private static readonly List<DemoUser> _users = new()
    {
        new("buyer1",  "pass123", "Alice Johnson",  "Buyer",  "alice.johnson@demo.com"),
        new("seller1", "pass123", "Bob Smith",      "Seller", "bob.smith@demo.com"),
        new("admin1",  "pass123", "Carol Williams", "Admin",  "carol.williams@demo.com"),
    };

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "Username and password are required." });

        var user = _users.FirstOrDefault(u =>
            u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase) &&
            u.Password == request.Password);

        if (user is null)
            return Unauthorized(new { message = "Invalid username or password." });

        return Ok(new LoginResponse(
            Token: $"demo-token-{Guid.NewGuid():N}",
            Username: user.Username,
            FullName: user.FullName,
            Role: user.Role,
            Email: user.Email
        ));
    }
}

public record LoginRequest(string Username, string Password);

public record LoginResponse(string Token, string Username, string FullName, string Role, string Email);

internal record DemoUser(string Username, string Password, string FullName, string Role, string Email);
