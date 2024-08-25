using Dashboard.Controllers;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services
            .AddAuthentication("Api")
            .AddScheme<AuthenticationSchemeOptions, AuthHandler>("Api", options => { });

// Add session.
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add CORS.
builder.Services.AddCors(options => {
    options.AddPolicy(
        "AllowAllOrigins",
        builder => {
            builder.AllowAnyOrigin() // Allowed all origins
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Enable CORS.
app.UseCors("AllowAllOrigins"); // Note: Required to call this method before "app.UseStaticFiles()"

app.UseHttpsRedirection();
app.UseStaticFiles();

// Redirect error to custom pages.
app.UseStatusCodePagesWithRedirects("/error/{0}");
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Enable routing.
app.UseRouting();

// Enable authentication.
app.UseAuthentication();
app.UseAuthorization();

// Enable session.
app.UseSession();

// Define routes.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}"
);

// Run application.
app.Run();
