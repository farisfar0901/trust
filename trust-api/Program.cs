using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Trust.Api.Application.Common;
using Trust.Api.Application.Repositories;
using Trust.Api.Application.Services.Authentication;
using Trust.Api.Application.Services.Events;
using Trust.Api.Application.Services.Gallery;
using Trust.Api.Application.Services.Volunteers;
using Trust.Api.Data;
using Trust.Api.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var bearerScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Paste the access token from POST /api/admin/auth/login. " +
            "Swagger adds the 'Bearer ' prefix automatically — do not include it yourself.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
    };
    options.AddSecurityDefinition("Bearer", bearerScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { { bearerScheme, Array.Empty<string>() } });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin());
});

builder.Services.AddDbContext<TrustDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("TrustDb");
    options.UseNpgsql(connectionString);
});

// Application layer: cross-cutting infrastructure.
builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
builder.Services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
builder.Services.AddSingleton<ISecureTokenGenerator, SecureTokenGenerator>();
builder.Services.AddScoped<IAccessTokenGenerator, HmacAccessTokenGenerator>();

// Application layer: repositories (Admin Authentication, Volunteer Requests, Events, Gallery).
builder.Services.AddScoped<IAdminUserRepository, AdminUserRepository>();
builder.Services.AddScoped<IAdminAuthTokenRepository, AdminAuthTokenRepository>();
builder.Services.AddScoped<IVolunteerRequestRepository, VolunteerRequestRepository>();
builder.Services.AddScoped<IVolunteerRepository, VolunteerRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IGalleryMediaRepository, GalleryMediaRepository>();

// Application layer: services (Admin Authentication, Volunteer Requests, Events, Gallery).
builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();
builder.Services.AddScoped<IVolunteerRequestService, VolunteerRequestService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IGalleryMediaService, GalleryMediaService>();

// JWT bearer authentication. Validates access tokens issued by
// HmacAccessTokenGenerator, so the signing key and issuer must match
// exactly what that generator uses (see appsettings "Jwt" section).
var jwtSigningKey = builder.Configuration["Jwt:SigningKey"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "TrustAdminApi";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // The default handler remaps short JWT claim names ("sub", "email",
        // "role") to long legacy XML-Soap claim URIs. Disabling that keeps
        // claim types exactly as HmacAccessTokenGenerator issued them.
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey ?? string.Empty)),
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            RoleClaimType = "role",
            NameClaimType = "email",
            ClockSkew = TimeSpan.FromSeconds(30),
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
