using ContentTokens.Extensions;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Web.Routing;

var builder = WebApplication.CreateBuilder(args);

// Add Optimizely CMS services
builder.Services
    .AddCms()
    .AddCmsAspNetIdentity<ApplicationUser>();

// Add MVC and Razor Pages
builder.Services.AddMvc();
builder.Services.AddRazorPages();

// Add Blazor Server support for the ContentTokens addon
builder.Services.AddServerSideBlazor();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Add ContentTokens middleware AFTER authentication/authorization
app.UseContentTokens();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapRazorPages();
    endpoints.MapBlazorHub();
    
    // CMS routing
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
