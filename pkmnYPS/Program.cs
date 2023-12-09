using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using pkmnYPS.PokeAPI;
using pkmnYPS.Repository;
using pkmnYPS.Services;
using pkmnYPS.Services.Interfaces.PokeAPI;
using pkmnYPS.Services.Interfaces.Repository;
using pkmnYPS.Services.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace pkmnYPS;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<PkmnContext>(
            options => options.UseMySQL(builder.Configuration.GetConnectionString("pkmnYPS") ?? string.Empty,
            b => b.MigrationsAssembly("pkmnYPS")));

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = builder.Configuration["Authentication:JwtIssuer"],
                    ValidAudience = builder.Configuration["Authentication:JwtAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:JwtKey"]!)),
                };
            });

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserSearchRepository, UserSearchRepository>();

        builder.Services.AddScoped<IPokemonFetcherService, PokemonFetcherService>();
        builder.Services.AddScoped<IAuthService, AuthService>();

        builder.Services.AddScoped<IPokeAPI, PokeAPIService>();

        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "pkmnYPS API", Version = "v1" });
            s.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Description = """
                    JWT Authorization header using the Bearer scheme.
                    Enter 'Bearer' [space] and then your token in the text input below.
                    Example: 'Bearer 12345abcdef'
                    """,
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            s.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });

        builder.Services.AddHttpClient<IPokeAPI, PokeAPIService>();

        builder.Services.AddCors(config => config.AddPolicy("allowedOrigins", policy => policy.WithOrigins("*")));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors(builder =>
            builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
