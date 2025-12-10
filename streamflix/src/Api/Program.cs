using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Streamflix.Infrastructure.Data;
using Streamflix.Api.Services;
using Streamflix.Api.Settings;
using Streamflix.Api.Formatting;

var builder = WebApplication.CreateBuilder(args);

builder.Services

    .AddControllers(options =>
    {
        options.OutputFormatters.Add(new CsvOutputFormatter()); // Support CSV responses
    })
    .AddXmlSerializerFormatters(); // Support XML responses

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IContentService, ContentService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IWatchlistService, WatchlistService>();
builder.Services.AddScoped<IViewingHistoryService, ViewingHistoryService>();


builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("JwtSettings");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwt["Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["SecretKey"]!)
            ),

            ValidateLifetime = true
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Streamflix API", Version = "v1" });

    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer {token}'",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new Microsoft.OpenApi.Models.OpenApiReference
        {
            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        { securityScheme, System.Array.Empty<string>() }
    };

    c.AddSecurityRequirement(securityRequirement);
});
builder.Services.AddScoped<IReferralService, ReferralService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Streamflix API V1");
    c.RoutePrefix = string.Empty;
});

// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<ApplicationDbContext>();

    const int maxRetries = 10;
    const int delayMs = 2000;

    for (var attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            logger.LogInformation("Attempt {Attempt}/{MaxRetries}: Applying migrations...", attempt, maxRetries);
            context.Database.Migrate();
            logger.LogInformation("Migrations applied. Seeding database...");
            DbSeeder.Seed(context);
            logger.LogInformation("Database seeding completed.");
            break;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while migrating/seeding the database on attempt {Attempt}.", attempt);

            if (attempt == maxRetries)
            {
                logger.LogCritical("Max retries reached. Giving up on database migration/seeding.");
                throw;
            }

            Thread.Sleep(delayMs);
        }
    }
}

app.Urls.Add("http://0.0.0.0:5001");
app.Run();
