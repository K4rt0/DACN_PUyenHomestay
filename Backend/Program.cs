using Backend.DataAccess;
using Backend.Extensions;
using Microsoft.EntityFrameworkCore;
using DotEnv.Core;
using Backend.Services;
using Backend.Repositories;

new EnvLoader().Load();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(Utils.DB_MYSQL, ServerVersion.AutoDetect(Utils.DB_MYSQL)));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserService, UserRepository>();

builder.Services.AddScoped<JwtAuthorizeAttribute>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();