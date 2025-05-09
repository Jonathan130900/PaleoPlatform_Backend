using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PaleoPlatform_Backend.Data;
using PaleoPlatform_Backend.Helpers;
using PaleoPlatform_Backend.Models;
using PaleoPlatform_Backend.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // The URL of the frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
})
.AddJwtBearer("JwtBearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "PaleoPlatformAPI",
        ValidAudience = "PaleoPlatformClient",
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("SuperSecretKeyThatIsLongEnough123456789")),
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var token = JwtHelpers.GetTokenFromRequest(context.HttpContext.Request);

            if (string.IsNullOrEmpty(token))
            {
                context.Fail("No token provided");
                return;
            }

            var dbContext = context.HttpContext.RequestServices
                .GetRequiredService<ApplicationDbContext>();

            if (await dbContext.ExpiredTokens.AnyAsync(t => t.Token == token))
            {
                context.Fail("Token has been invalidated");
            }
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHostedService<TokenCleanupService>();

// Configure Swagger with file upload support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PaleoPlatform API", Version = "v1" });

    // Handle file uploads
    c.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });

    // Security definitions
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    c.OperationFilter<FileUploadOperationFilter>();
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Custom services
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IArticoloService, ArticoloService>();
builder.Services.AddScoped<IUtenteService, UtenteService>();
builder.Services.AddScoped<IStripeService, StripeService>();

 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaleoPlatform API V1");
    });
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "uploads")),
    RequestPath = "/uploads",
    ContentTypeProvider = new FileExtensionContentTypeProvider
    {
        Mappings = {
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".webp"] = "image/webp"
        }
    },
    ServeUnknownFileTypes = true, // Temporary for debugging
    OnPrepareResponse = ctx =>
    {
        // Force correct MIME types
        var fileExt = Path.GetExtension(ctx.File.Name).ToLower();
        ctx.Context.Response.Headers["Cache-Control"] = "no-cache";

        if (fileExt == ".jpg" || fileExt == ".jpeg")
            ctx.Context.Response.ContentType = "image/jpeg";
        else if (fileExt == ".webp")
            ctx.Context.Response.ContentType = "image/webp";
    }
});
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Seed Roles and Admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbSeeder.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Seeding error: {ex.Message}");
    }
}

app.Run();

// Class declarations
public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileParams = context.MethodInfo.GetParameters()
            .Where(p => p.ParameterType == typeof(IFormFile))
            .ToList();

        if (fileParams.Count > 0)
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Content =
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties =
                            {
                                ["file"] = new OpenApiSchema
                                {
                                    Description = "Select file",
                                    Type = "string",
                                    Format = "binary"
                                }
                            }
                        }
                    }
                }
            };

            operation.Parameters.Clear();
        }
    }
}
