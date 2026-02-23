var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpClient("IdentityAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["IdentityApi:BaseUrl"] ?? "http://localhost:5100");
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.MapRazorPages();

// Redirect root to login
app.MapGet("/", () => Results.Redirect("/Login"));

app.Run();
