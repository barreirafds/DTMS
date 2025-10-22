using DTMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using DTMS.Data.Models;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<AppDbContext>(o =>
//    o.UseMySql(cs, ServerVersion.AutoDetect(cs)));

builder.Services.AddRazorPages();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.Run();
