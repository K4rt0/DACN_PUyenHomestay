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

builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "KartoCorsPolicy",
                      policy =>
                      {
                          policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
                      });
});

builder.Services.AddScoped<IUserService, UserRepository>();

builder.Services.AddScoped<IBranchService, BranchRepository>();
builder.Services.AddScoped<IBranchContactService, BranchContactRepository>();
builder.Services.AddScoped<IBranchContactDetailService, BranchContactDetailRepository>();

builder.Services.AddScoped<IVietnamProvinceService, VietnamProvinceRepository>();
builder.Services.AddScoped<JwtAuthorizeAttribute>();

builder.Services.AddMemoryCache();
builder.Services.AddScoped<RateLimitFilter>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("KartoCorsPolicy");
app.UseAuthorization();
app.MapControllers();
app.Run();