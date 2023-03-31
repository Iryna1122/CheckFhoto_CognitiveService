using CheckFhoto_CognitiveService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.Cosmos;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);


//string connection = builder.Configuration.GetConnectionString("DefaulltConnection"); // My DtaBase

//builder.Services.AddDbContext<AppContextt>(options => options.UseSqlServer(connection));


builder.Services.AddDbContext<AppContextt>(options => options.UseCosmos("AccountEndpoint=https://oliinykstore.documents.azure.com:443/;AccountKey=7PGmhxtUiLnl9fr4h3lvWod3Lk3Jj0CynvfbDWzMfF8YlgL3MmEj4YbMBlrG8wlmCCaDMQPxYBgGACDbcArwHQ==;", "MyPhoto"));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
