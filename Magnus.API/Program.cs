using Magnus.API.Data;
using Magnus.API.Helpers;
using Magnus.API.Mappings;
using Magnus.API.Middleware;
using Magnus.API.Models;
using Magnus.API.Services.Implementations;
using Magnus.API.Services.Interfaces;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services
    .AddOptions<SeedSettings>()
    .Bind(builder.Configuration.GetSection(SeedSettings.SectionName))
    .ValidateDataAnnotations()
    .Validate(
        settings => !settings.AdminPassword.Contains("__SET_IN_USER_SECRETS_OR_ENV__", StringComparison.Ordinal),
        "SeedSettings:AdminPassword must be provided through user secrets or environment variables.")
    .ValidateOnStart();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Validation error." : e.ErrorMessage)
            .ToArray();

        return new BadRequestObjectResult(ApiResponse<object>.FailureResponse("Validation failed.", errors, context.HttpContext.TraceIdentifier));
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Magnus API",
        Version = "v1",
        Description = "Production-style employee management API with Identity, JWT authentication, auditing, and role-based access."
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer {token}'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath, true);
    }
});

var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(defaultConnection))
{
    throw new InvalidOperationException("DefaultConnection must be provided through configuration, user secrets, or environment variables.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options
        .UseNpgsql(defaultConnection)
        .ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning)));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 10;
        options.Password.RequiredUniqueChars = 3;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName);
var secret = jwtSettings["Secret"];

if (string.IsNullOrWhiteSpace(secret))
{
    throw new InvalidOperationException("JWT secret must be provided through configuration or user secrets.");
}

if (Encoding.UTF8.GetByteCount(secret) < 32)
{
    throw new InvalidOperationException("JWT secret must be at least 32 bytes for HMAC-SHA256 signing.");
}

var accessTokenExpiryMinutes = jwtSettings.GetValue<int>("ExpiryMinutes");
if (accessTokenExpiryMinutes < 15 || accessTokenExpiryMinutes > 30)
{
    throw new InvalidOperationException("JwtSettings:ExpiryMinutes must be between 15 and 30 minutes for production token hygiene.");
}

var seedSettings = builder.Configuration.GetSection(SeedSettings.SectionName);
if (string.IsNullOrWhiteSpace(seedSettings["AdminPassword"]))
{
    throw new InvalidOperationException("SeedSettings:AdminPassword must be provided through configuration, user secrets, or environment variables.");
}

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
var isRender = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("RENDER"));

builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = key,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(RoleConstants.Admin));
    options.AddPolicy("AdminOrManager", policy => policy.RequireRole(RoleConstants.Admin, RoleConstants.Manager));
});

var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactClient", policy =>
    {
        if (corsOrigins.Length == 0)
        {
            return;
        }

        policy.WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfile).Assembly);
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDesignationService, DesignationService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

var app = builder.Build();

await ApplyMigrationsAndSeedAsync(app);

app.UseForwardedHeaders();
app.UseMiddleware<ExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "swagger";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Magnus API v1");
});

if (!isRender)
{
    app.UseHttpsRedirection();
}

app.UseCors("ReactClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();

static async Task ApplyMigrationsAndSeedAsync(WebApplication app)
{
    await using var scope = app.Services.CreateAsyncScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    app.Logger.LogInformation("Applying pending database migrations.");
    await context.Database.MigrateAsync();
    app.Logger.LogInformation("Database migrations applied successfully.");

    await using var connection = context.Database.GetDbConnection();
    await connection.OpenAsync();
    app.Logger.LogInformation("Database connection verified successfully.");
    await connection.CloseAsync();

    await DbSeeder.SeedIdentityAsync(services, app.Logger);
    app.Logger.LogInformation("Identity roles and admin seeding completed successfully.");
}
