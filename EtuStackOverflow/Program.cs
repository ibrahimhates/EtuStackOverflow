using AskForEtu.Core.Map;
using EtuStackOverflow.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.JwtSettings.json");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();

// dbContext yapilandirilmasi connection string baglantisi
builder.Services.ConfigureDbContext(builder.Configuration);

// Automapper added
builder.Services.AddAutoMapper(typeof(Mapper));

//Service Repo Bagimlilik Kayitlari
builder.Services.ConfigureServices();
builder.Services.ConfigureRepos();
builder.Services.ConfigureResponsibility();

//JwtBearer configurasyonlari
builder.Services.ConfigureJwtBearer(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
