using Dermainsight.Filters;
using Dermainsight.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

// CookieService servisini ekle
builder.Services.AddScoped<FastApiStatusCheckFilter>();
builder.Services.AddHttpClient<FastApiService>();

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<FastApiStatusCheckFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();
