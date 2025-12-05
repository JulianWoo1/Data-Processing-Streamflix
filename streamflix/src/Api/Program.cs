using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Commented out because it was causing issues with some requests during development
// app.UseHttpsRedirection();

app.MapControllers();

// Wait for DB to be ready, then migrate + seed
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

app.Run();
