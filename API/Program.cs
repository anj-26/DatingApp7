using API.Data;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(optionsAction: opt =>
{
 opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyHeader().WithOrigins("https://localhost:4200"));
app.MapControllers(); //middleware to map controller endpoints

app.Run();
