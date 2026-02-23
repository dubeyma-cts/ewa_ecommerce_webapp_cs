using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EWA.Web.Pages;

public class HomeModel : PageModel
{
    public string FullName { get; private set; } = string.Empty;
    public string Role     { get; private set; } = string.Empty;
    public string Email    { get; private set; } = string.Empty;

    // Hardcoded demo categories
    public List<(string Icon, string Name)> Categories { get; } = new()
    {
        ("🏺", "Antiques"),     ("🎨", "Art"),          ("🍼", "Baby Products"),
        ("📚", "Books"),        ("📷", "Cameras"),       ("📱", "Mobile Phones"),
        ("📀", "DVDs"),         ("🧸", "Toys"),          ("💻", "Computers"),
        ("⌚", "Watches"),       ("💍", "Jewelry"),       ("📺", "Electronics"),
        ("🏠", "Home Appliances"),
    };

    // Hardcoded top 5 demo items
    public List<DemoItem> TopItems { get; } = new()
    {
        new("📱", "iPhone 15 Pro",        999.99m, "Buy Now"),
        new("💻", "MacBook Air M3",       1299.00m, "Auction"),
        new("📷", "Sony A7 IV Camera",     2499.00m, "Auction"),
        new("⌚", "Rolex Submariner",     8950.00m, "Bid"),
        new("🎨", "Monet Print (Signed)", 350.00m, "Buy Now"),
    };

    public IActionResult OnGet()
    {
        var fullName = HttpContext.Session.GetString("FullName");
        if (fullName is null)
            return RedirectToPage("/Login");

        FullName = fullName;
        Role     = HttpContext.Session.GetString("Role")  ?? "Buyer";
        Email    = HttpContext.Session.GetString("Email") ?? string.Empty;
        return Page();
    }

    public IActionResult OnPostLogout()
    {
        HttpContext.Session.Clear();
        return RedirectToPage("/Login");
    }

    public record DemoItem(string Emoji, string Name, decimal Price, string Tag);
}
