using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;      
using Streamflix.Infrastructure.Data;
using Streamflix.Api.Services;
using Streamflix.Api.Settings;             

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Register services
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Register JWT settings
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<JwtSettings>>().Value);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Streamflix API V1");
    c.RoutePrefix = string.Empty; 
});

app.UseAuthorization();
app.MapControllers();

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

app.Urls.Add("http://0.0.0.0:5001");
app.Run();
