using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using PersistenceService.Data.ApplicationDb;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer(
        "Bearer",
        options =>
        {
            options.Authority = "https://localhost:5001";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false
            };
        }
    );
DotNetEnv.Env.Load();
string connectionString =
    Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "";
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(connectionString)
);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
