using BlazorBasic;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

// var builder = WebAssemblyHostBuilder.CreateDefault(args);
// builder.RootComponents.Add<App>("#app");
// builder.RootComponents.Add<HeadOutlet>("head::after");

// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// await builder.Build().RunAsync();

using Heist.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

var rng = new Random();

// API: return 3 options with colors "red" or "black"
app.MapGet("/api/options", () =>
{
    var options = new List<Option>
    {
        new Option { Id = 1, Color = rng.Next(2) == 0 ? "red" : "black" },
        new Option { Id = 2, Color = rng.Next(2) == 0 ? "red" : "black" },
        new Option { Id = 3, Color = rng.Next(2) == 0 ? "red" : "black" },
    };
    return Results.Json(options);
});

// API: receive a selection { "color": "red" } and log it
app.MapPost("/api/selection", async (HttpContext ctx) =>
{
    var dto = await ctx.Request.ReadFromJsonAsync<SelectionDto>();
    if (dto == null || string.IsNullOrWhiteSpace(dto.Color))
        return Results.BadRequest();

    app.Logger.LogInformation("Received selection: {color}", dto.Color);

    // Respond with acknowledgement
    return Results.Ok(new { received = dto.Color });
});

// Blazor endpoints
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

// small DTO record for POST body
internal record SelectionDto(string Color);
