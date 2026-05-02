using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PVS.Api.Data;
using System.Text;
using PVS.Api.Common;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))
    )
);

// Add services
builder.Services.AddControllers();
builder.Services.AddScoped<DbSeeder>();

// Repositories
builder.Services.AddScoped<PVS.Api.Common.Repository.IGenericRepository<PVS.Api.Models.User>, PVS.Api.Common.Repository.GenericRepository<PVS.Api.Models.User>>();
builder.Services.AddScoped<PVS.Api.Modules.Auth.Repository.IUserRepository, PVS.Api.Modules.Auth.Repository.UserRepository>();
builder.Services.AddScoped<PVS.Api.Modules.Branches.Repository.IBranchRepository, PVS.Api.Modules.Branches.Repository.BranchRepository>();
builder.Services.AddScoped<PVS.Api.Modules.Clients.Repository.IClientRepository, PVS.Api.Modules.Clients.Repository.ClientRepository>();
builder.Services.AddScoped<PVS.Api.Modules.Properties.Repository.IPropertyRepository, PVS.Api.Modules.Properties.Repository.PropertyRepository>();
builder.Services.AddScoped<PVS.Api.Modules.Appointments.Repository.IAppointmentRepository, PVS.Api.Modules.Appointments.Repository.AppointmentRepository>();
builder.Services.AddScoped<PVS.Api.Modules.Offers.Repository.IOfferRepository, PVS.Api.Modules.Offers.Repository.OfferRepository>();

// Services
builder.Services.AddScoped<PVS.Api.Modules.Auth.Services.IAuthService, PVS.Api.Modules.Auth.Services.AuthService>();
builder.Services.AddScoped<PVS.Api.Modules.Properties.Services.IPropertiesService, PVS.Api.Modules.Properties.Services.PropertiesService>();
builder.Services.AddScoped<PVS.Api.Modules.Clients.Services.IClientService, PVS.Api.Modules.Clients.Services.ClientService>();
builder.Services.AddScoped<PVS.Api.Modules.Appointments.Services.IAppointmentService, PVS.Api.Modules.Appointments.Services.AppointmentService>();
builder.Services.AddScoped<PVS.Api.Modules.Offers.Services.IOfferService, PVS.Api.Modules.Offers.Services.OfferService>();

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secret = jwtSettings["Secret"];
if (string.IsNullOrWhiteSpace(secret))
    throw new Exception("JwtSettings:Secret is missing from configuration");
var key = Encoding.UTF8.GetBytes(secret);
Console.WriteLine("VALIDATION SECRET: " + builder.Configuration["JwtSettings:Secret"]);
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            //ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine("JWT FAILED: " + ctx.Exception.ToString());
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                Console.WriteLine("JWT VALIDATED");
                return Task.CompletedTask;
            },
            OnChallenge = ctx =>
            {
                Console.WriteLine($"JWT CHALLENGE: {ctx.Error} | {ctx.ErrorDescription}");
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
    
    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await DbSeeder.SeedAsync(dbContext);
}


app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.MapGet("/api/health", () => new { status = "ok", timestamp = DateTime.UtcNow })
    .WithName("HealthCheck");

app.Run();
