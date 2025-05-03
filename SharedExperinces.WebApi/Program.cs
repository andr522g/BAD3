using AuthDemo.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SharedExperinces.WebApi.DataAccess;
using SharedExperinces.WebApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString = "Data Source=127.0.0.1,1433;Database=SharedExperincesDB;User Id=sa;Password=Password,1;TrustServerCertificate=True";
  

Console.WriteLine($"Connection string: {connectionString}");

builder.Services.AddDbContext<SharedExperinceContext>(options =>
	options.UseSqlServer(connectionString));


builder.Services.AddIdentity<ApiUser, IdentityRole>()
    .AddEntityFrameworkStores<SharedExperinceContext>();

//  JWT
builder.Services.AddAuthentication("Jwt")
    .AddJwtBearer("Jwt", o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
            ValidateLifetime = true
        };
    });

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthorization();   // we’ll add policies later

builder.Services.AddTransient<ServiceService>();
builder.Services.AddTransient<SharedExperienceService>();
builder.Services.AddTransient<ProviderService>();

var app = builder.Build();


/* app.Lifetime.ApplicationStarted.Register(async () =>
{
    using var scope = app.Services.CreateScope();
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApiUser>>();

    if (!await roleMgr.RoleExistsAsync("Admin"))
        await roleMgr.CreateAsync(new IdentityRole("Admin"));

    var admin = await userMgr.FindByEmailAsync("admin@demo.com");
    if (admin == null)
    {
        admin = new ApiUser { UserName = "admin@demo.com", Email = "admin@demo.com" };
        await userMgr.CreateAsync(admin, "Admin123$");
        await userMgr.AddToRoleAsync(admin, "Admin");
    }
}); */

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<SharedExperinceContext>();

	dbContext.Database.Migrate(); 

	SharedExperinceContext.Seed(dbContext); 
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
