using Microsoft.EntityFrameworkCore;
using Streamflix.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger (Docker-friendly)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    // Absolute URL ensures Swagger works inside Docker
    c.SwaggerEndpoint("http://localhost:5001/swagger/v1/swagger.json", "Streamflix API V1");
    c.RoutePrefix = string.Empty; // Serve Swagger at root
});

app.UseAuthorization();
app.MapControllers();

// Retry loop for database migration
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    var retries = 10;
    while (retries > 0)
    {
        try
        {
            context.Database.Migrate();
            break;
        }
        catch
        {
            retries--;
            Console.WriteLine("Waiting for DB to be ready...");
            System.Threading.Thread.Sleep(2000);
        }
    }

    DbSeeder.Seed(context);
}

// Listen on all interfaces for Docker
app.Urls.Add("http://0.0.0.0:5001");

app.Run();
