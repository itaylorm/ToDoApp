using BlazorApiClient.Components;
using BlazorApiClient.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddTransient<TokenModel>();

var apiUrl = builder.Configuration.GetValue<string>("ApiUrl");
if(string.IsNullOrEmpty(apiUrl)) {
    throw new Exception("ApiUrl is required");
}

builder.Services.AddHttpClient("api", opts =>
{
    opts.BaseAddress = new Uri(apiUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
