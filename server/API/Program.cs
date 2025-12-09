using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

var builder = WebApplication.CreateBuilder(args);

// 1. Setup Database Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CasinoContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 2. Allow the Website to talk to us (CORS)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowAll");

// 3. The Endpoint: Website asks for "/api/rng", we give them the number
app.MapGet("/api/rng", async (CasinoContext db) =>
{
    // Get the first number from the database
    var number = await db.RandomNumbers.FirstOrDefaultAsync();
    return number != null ? number : new RandomNumber { Value = 0 };
});

app.Run();

// --- DATA MODELS ---

// This matches your "RandomNumbers" table in AWS
public class RandomNumber
{
    public int Id { get; set; }
    public int Value { get; set; }
}

// This tells C# how to talk to the database
public class CasinoContext : DbContext
{
    public CasinoContext(DbContextOptions<CasinoContext> options) : base(options) { }
    public DbSet<RandomNumber> RandomNumbers { get; set; }
}