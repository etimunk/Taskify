using Taskify.Core;
using Taskify.Core.Repositories;
using Taskify.Core.Servieces;
using Taskify.Data;
using Taskify.Data.Repositories;
using Taskify.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Linq;
using System.Security.Claims;
using System.Net.NetworkInformation;

var builder = WebApplication.CreateBuilder(args);

// Prevent noisy crash when a second dev instance starts on the same port.
if (builder.Environment.IsDevelopment())
{
    var devPort = 5233;
    var isPortInUse = IPGlobalProperties.GetIPGlobalProperties()
        .GetActiveTcpListeners()
        .Any(endpoint => endpoint.Port == devPort);

    if (isPortInUse)
    {
        Console.WriteLine($"Development server is already running on http://localhost:{devPort}. Stop the existing instance before starting a new one.");
        return;
    }
}

// Add services to the container.
builder.Services.AddControllers();

// Add services related to Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Taskify API", Version = "v1" });
    options.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.Name);
    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Put ONLY your token in the box below (without 'Bearer ' prefix)",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

// Dependency Injection
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddDbContext<DataContext>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Taskify.MappingAPI), typeof(Taskify.Core.MappingProfile));

// Add Authentication with JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // תיקון: בדיקה שהמפתח קיים לפני שימוש
    var jwtKey = builder.Configuration["JWT:Key"];
    if (string.IsNullOrEmpty(jwtKey))
    {
        var debugKey = builder.Configuration["JWT:Key"];
        Console.WriteLine($"DEBUG: The value found for JWT:Key is: '{debugKey}'");
        throw new InvalidOperationException("JWT Key is missing in appsettings.json! Please check your configuration.");
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.Email
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
        policy.WithOrigins("http://localhost:8080", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();
app.UseCors("AllowReact");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// חשוב: Authentication תמיד לפני Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();

    if (!context.users.Any())
    {
        context.users.Add(new Taskify.Core.Entities.User
        {
            TZ = "000000000",
            Name = "Admin",
            Email = "admin@taskify.com",
            Password = "Admin123!",
            Role = "headmanager",
            Level = Taskify.Core.Entities.User.IsManager.headmanager
        });
        context.SaveChanges();
    }
}

app.Run();