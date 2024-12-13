using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using social_oc_api.Data;
using social_oc_api.Mappings;
using social_oc_api.Models.Domain;
using social_oc_api.Repositories;
using social_oc_api.Repositories.User;
using social_oc_api.Utils;
using System.Text;
using social_oc_api.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:5173", "http://172.28.240.1:5173")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

// Agregar servicios a la aplicación
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true; // Opcional para mejor formato
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDataProtection();
builder.Services.AddSignalR();

// Configurar el DbContext antes de construir la aplicación
builder.Services.AddDbContext<SocialOCDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar Identity con la entidad ApplicationUser
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<SocialOCDBContext>()
    .AddDefaultTokenProviders();

// Configuración de opciones de identidad
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

// Configuración de autenticación
builder.Services.AddAuthentication(options =>
{
    // Esquema por defecto (JWT)
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.Zero
    };
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Google:ClientId"];
    options.ClientSecret = builder.Configuration["Google:ClientSecret"];
    options.CallbackPath = "/api/auth/google/callback/";
});

// Inyectar dependencias personalizadas
builder.Services.AddScoped<ITokenRepository, SQLTokenRepository>();
builder.Services.AddScoped<IPostRepository, SQLPostRepository>();
builder.Services.AddScoped<IImageRepository, SQLImageRepository>();
builder.Services.AddScoped<IFollowerRepository, SQLFollowerRepository>();
builder.Services.AddScoped<IUserRepository, SQLUserRepository>();

builder.Services.AddScoped<IUtils, Utils>();

//REGISTERING MAPPING ==========================================
builder.Services.AddAutoMapper(typeof(MainAutoMapper));
// ================================================================

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "social-oc-api",
        Version = "v1"
    });

    // Definir esquema de seguridad
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Introduce el token JWT en el formato: Bearer {token}"
    });

    // Requerir el esquema de seguridad por defecto
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


var app = builder.Build();


// Configurar middlewares en el pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors();


// Configuration to save IMAGES/STATIC FILES  =================================´
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
    RequestPath = "/Images"
});
// ====================================================================

// Mapea los controladores
app.MapControllers();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("¡La aplicación se ha iniciado correctamente!");
app.MapHub<ChatHub>("/chatHub");

app.Run();
