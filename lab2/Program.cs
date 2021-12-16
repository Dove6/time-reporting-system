using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TRS.DataManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TrsDbContext>(options => options.UseSqlite("Data Source=storage/trs.db"));
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IDataManager>(x => new DbDataManager(x.GetRequiredService<TrsDbContext>(), x.GetRequiredService<IMapper>()));

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

app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.MapFallback(httpContext =>
{
    httpContext.Response.Redirect("/Home/Index");
    return Task.CompletedTask;
});

using (var serviceScope = app.Services.CreateScope())
    serviceScope.ServiceProvider.GetService<TrsDbContext>()!.Database.EnsureCreated();

app.Run();
