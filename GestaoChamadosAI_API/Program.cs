using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AspNetCoreRateLimit;
using Serilog;
using GestaoChamadosAI_API.Data;
using GestaoChamadosAI_API.Services;
using GestaoChamadosAI_API.Helpers;
using GestaoChamadosAI_API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/api-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Configurações do appsettings
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

// Configurar DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")));

// Adicionar Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Configurar HttpClient
builder.Services.AddHttpClient();

// Registrar Serviços
builder.Services.AddScoped<PasswordHashService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<IAService>();
builder.Services.AddSingleton<GeminiService>();

// Configurar Autenticação JWT
var secretKey = jwtSettings["SecretKey"] ?? throw new ArgumentNullException("JWT SecretKey não configurada");
var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Em produção, mude para true
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Configurar Autorização
builder.Services.AddAuthorization();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configurar Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Gestão de Chamados AI - API",
        Version = "v1",
        Description = "API REST para sistema de gestão de chamados com integração de Inteligência Artificial (Google Gemini)",
        Contact = new OpenApiContact
        {
            Name = "Equipe de Desenvolvimento",
            Email = "dev@gestaochamados.com"
        }
    });

    // Configurar autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu token}"
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
});

var app = builder.Build();

// Middleware Pipeline
// Habilitar Swagger em todos os ambientes
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestão de Chamados AI - API v1");
    c.RoutePrefix = "swagger"; // Swagger em /swagger
});

// Middleware customizado de tratamento de erros
app.UseMiddleware<ErrorHandlingMiddleware>();

// Rate Limiting
app.UseIpRateLimiting();

app.UseHttpsRedirection();

// Arquivos estáticos - Servir da pasta compartilhada e local
var sharedUploadPath = builder.Configuration["UploadSettings:SharedUploadPath"];
if (!string.IsNullOrEmpty(sharedUploadPath) && Directory.Exists(sharedUploadPath))
{
    // Servir arquivos da pasta compartilhada (Web)
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(sharedUploadPath),
        RequestPath = "/uploads"
    });
    Log.Information($"📁 Servindo uploads da pasta compartilhada: {sharedUploadPath}");
}
else
{
    // Fallback: servir da pasta local
    app.UseStaticFiles();
    Log.Warning("⚠️ Pasta compartilhada não configurada, usando pasta local");
}

// CORS
app.UseCors("AllowAll");

// Autenticação e Autorização
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Log de inicialização
Log.Information("🚀 API iniciada com sucesso!");
Log.Information($"📊 Ambiente: {app.Environment.EnvironmentName}");
Log.Information($"🌐 Swagger disponível em: http://localhost:5000");

app.Run();
