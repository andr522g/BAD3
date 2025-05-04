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
using System.Security.Claims;

// Initialize Serilog first
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog from appsettings.json
    builder.Host.UseSerilog((ctx, cfg) =>
    {
        cfg.ReadFrom.Configuration(ctx.Configuration);
        
        // Ensure MongoDB logging is enabled explicitly
        var mongoConnectionString = ctx.Configuration["MongoDbSettings:ConnectionString"] ?? "mongodb://localhost:27017";
        var mongoDatabaseName = ctx.Configuration["MongoDbSettings:DatabaseName"] ?? "LoggingDb";
        var mongoCollectionName = ctx.Configuration["MongoDbSettings:LogCollectionName"] ?? "logs";
        
        cfg.WriteTo.MongoDB(
            $"{mongoConnectionString}/{mongoDatabaseName}",
            mongoCollectionName);
    });

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var connectionString = "Data Source=127.0.0.1,1433;Database=SharedExperincesDB2;User Id=sa;Password=Zerefez7253!;TrustServerCertificate=True";

    Log.Debug("Using connection string: {ConnectionString}", connectionString);

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

    Log.Debug("JWT:Issuer = {Issuer}", builder.Configuration["JWT:Issuer"]);
    Log.Debug("JWT:Audience = {Audience}", builder.Configuration["JWT:Audience"]);
    Log.Debug("JWT:Key length = {KeyLength}", builder.Configuration["JWT:Key"]?.Length ?? 0);

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

    builder.Services.AddAuthorization();   // we'll add policies later

    builder.Services.AddTransient<ServiceService>();
    builder.Services.AddTransient<SharedExperienceService>();
    builder.Services.AddTransient<ProviderService>();
    builder.Services.AddSingleton<LogService>();        

    var app = builder.Build();

    app.UseCors("AllowAll");

    app.UseSerilogRequestLogging(opts =>
    {
        // Export method & path – the Filter in appsettings picks them up
        opts.EnrichDiagnosticContext = (diag, ctx) =>
        {
            diag.Set("RequestMethod", ctx.Request.Method);
            diag.Set("RequestPath", ctx.Request.Path);
            
            // Get user information if available
            if (ctx.User?.Identity?.IsAuthenticated == true)
            {
                diag.Set("UserName", ctx.User.Identity.Name);
                diag.Set("UserId", ctx.User.FindFirstValue(ClaimTypes.NameIdentifier));
                
                // Extract user roles
                var roles = ctx.User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();
                
                if (roles.Any())
                {
                    diag.Set("UserRole", string.Join(", ", roles));
                }
                
                // Get email if available
                var email = ctx.User.FindFirstValue(ClaimTypes.Email);
                if (!string.IsNullOrEmpty(email))
                {
                    diag.Set("UserEmail", email);
                }
            }
            
            diag.Set("RemoteIpAddress", ctx.Connection.RemoteIpAddress);
            diag.Set("RequestHost", ctx.Request.Host.Value);
            diag.Set("RequestScheme", ctx.Request.Scheme);
            
            // Add a description for the operation based on the request
            if (ctx.Request.Method is "POST" or "PUT" or "DELETE")
            {
                var operation = $"{ctx.Request.Method} {ctx.Request.Path}";
                diag.Set("Description", operation);
            }
        };
    });

    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SharedExperinceContext>();
            Log.Information("Running database migrations");
            dbContext.Database.Migrate();
            Log.Information("Seeding database");
            SharedExperinceContext.Seed(dbContext);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred during database migration");
        }
    }

    app.Lifetime.ApplicationStarted.Register(async () =>
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
    }); 
    
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { RoleNames.Admin, RoleNames.Manager, RoleNames.Provider, RoleNames.Guest };

            foreach (var roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                    Log.Information("Created role {RoleName}", roleName);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while creating roles");
        }
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseRouting();              

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    // Ensure proper logging shutdown
    Log.CloseAndFlush();
}
