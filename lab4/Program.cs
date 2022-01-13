using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Trs;
using Trs.DataManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TrsDbContext>(options => options.UseSqlite($"Data Source={TrsDbContext.DbPath}"));
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddControllers();
builder.Services.AddScoped<IDataManager>(x => new DbDataManager(x.GetRequiredService<TrsDbContext>(), x.GetRequiredService<IMapper>()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(options =>
{
    const string customSchemeName = "customScheme";
    options.AddScheme<DummyAuthHandler>(customSchemeName, "Custom scheme");
    options.DefaultScheme = customSchemeName;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllers();
app.MapFallbackToFile("index.html");

using (var serviceScope = app.Services.CreateScope())
    TrsDbInitializer.Initialize(serviceScope.ServiceProvider.GetService<TrsDbContext>()!);

app.Run();
