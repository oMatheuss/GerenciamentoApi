using Microsoft.EntityFrameworkCore;
using GerenciamentoAPI.Repository;
using GerenciamentoAPI.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GerenciamentoAPI.Helpers;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<GerenciamentoDB>(opt =>
{
    opt.UseNpgsql(builder.Configuration["ConnectionString"].ToString());
});

builder.Services.AddScoped<GerenciamentoService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GerenciamentoAPI", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "JWT Authorization header usando o esquema Bearer.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            Array.Empty<string>()
        }
    });
});

var jwtSecret = builder.Configuration["jwtSecret"].ToString();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        };
    });

builder.Services.AddAuthorization(auth =>
{
    auth.DefaultPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser().Build();
});

builder.Services.AddSingleton(new JWTManager(jwtSecret));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
