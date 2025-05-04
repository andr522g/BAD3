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
builder.Services.AddSwaggerGen();


var connectionString = "Data Source=127.0.0.1,1433;Database=SharedExperincesDB;User Id=sa;Password=Cefemivo+f113;TrustServerCertificate=True";
  

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

app.UseCors("AllowAll");


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





using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<SharedExperinceContext>();

	dbContext.Database.Migrate(); 

	SharedExperinceContext.Seed(dbContext); 
}


/*

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

*/

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
