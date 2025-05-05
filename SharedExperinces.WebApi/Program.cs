using AuthDemo.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using SharedExperinces.WebApi.DataAccess;
using SharedExperinces.WebApi.Models;
using SharedExperinces.WebApi.Services;
using System.Text;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration);
});


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SharedExperiences API",
        Version = "v1",
        Description = "SharedExperiences Web API"
    });

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

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var connectionString = "Data Source=sqlserver,1433;Database=SharedExperincesDB2;User Id=sa;Password=Zerefez7253!;TrustServerCertificate=True";
  

Console.WriteLine($"Connection string: {connectionString}");

builder.Services.Configure<LoggingDatabaseSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = builder.Configuration.GetSection("MongoDbSettings").Get<LoggingDatabaseSettings>();
    return new MongoClient(settings.ConnectionString);
});




builder.Services.AddDbContext<SharedExperinceContext>(options =>
	options.UseSqlServer(connectionString));


builder.Services.AddIdentity<ApiUser, IdentityRole>()
    .AddEntityFrameworkStores<SharedExperinceContext>();








Console.WriteLine("JWT:Issuer = " + builder.Configuration["JWT:Issuer"]);
Console.WriteLine("JWT:Audience = " + builder.Configuration["JWT:Audience"]);
Console.WriteLine("JWT:Key = " + builder.Configuration["JWT:Key"]);


builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["JWT:Issuer"],
		ValidAudience = builder.Configuration["JWT:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
	};
});


builder.Services.AddAuthorization();   // we'll add policies later

builder.Services.AddTransient<ServiceService>();
builder.Services.AddTransient<SharedExperienceService>();
builder.Services.AddTransient<ProviderService>();
builder.Services.AddSingleton<LogService>();        




var app = builder.Build();

app.UseCors("AllowAll");

// Add root path redirection to Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// Always enable Swagger in all environments
app.UseSwagger(c => {
    c.RouteTemplate = "swagger/{documentName}/swagger.json";
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SharedExperiences API V1");
    c.RoutePrefix = "swagger";
});

// Only use HTTPS redirection in development, not in Docker
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts();
}

app.UseSerilogRequestLogging(opts =>
{
    // Export method & path – the Filter in appsettings picks them up
    opts.EnrichDiagnosticContext = (diag, ctx) =>
    {
        diag.Set("RequestMethod", ctx.Request.Method);
        diag.Set("RequestPath", ctx.Request.Path);
        diag.Set("UserName", ctx.User.Identity?.Name);
    };
});





try
{
	using (var scope = app.Services.CreateScope())
	{
		var dbContext = scope.ServiceProvider.GetRequiredService<SharedExperinceContext>();

		try
		{
			dbContext.Database.Migrate(); // Apply migrations
			SharedExperinceContext.Seed(dbContext); // Seed data
		}
		catch (Exception dbEx)
		{
			// Log the error but don't rethrow
			Console.WriteLine($"Database initialization failed: {dbEx.Message}");
			Console.WriteLine(dbEx.StackTrace);
		}
	}
}
catch (Exception ex)
{
	// Log outer scope error (e.g. DI failure)
	Console.WriteLine($"Service scope or DbContext resolution failed: {ex.Message}");
	Console.WriteLine(ex.StackTrace);
}






app.Lifetime.ApplicationStarted.Register(async () =>
{
	try
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
			var result = await userMgr.CreateAsync(admin, "Admin123$");

			if (result.Succeeded)
			{
				await userMgr.AddToRoleAsync(admin, "Admin");
			}
			else
			{
				Console.WriteLine("❌ Failed to create admin user:");
				foreach (var error in result.Errors)
					Console.WriteLine($"   - {error.Description}");
			}
		}
	}
	catch (Exception ex)
	{
		Console.WriteLine("❌ Error during admin user/role setup:");
		Console.WriteLine(ex.Message);
		Console.WriteLine(ex.StackTrace);
	}
}); 
 




using (var scope = app.Services.CreateScope())
{
	var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

	string[] roleNames = { RoleNames.Admin, RoleNames.Manager, RoleNames.Provider, RoleNames.Guest };

	foreach (var roleName in roleNames)
	{
		var roleExists = await roleManager.RoleExistsAsync(roleName);
		if (!roleExists)
		{
			await roleManager.CreateAsync(new IdentityRole(roleName));
		}
	}
}




app.UseRouting();              

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
