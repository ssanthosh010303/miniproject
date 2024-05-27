using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using WebApi.Authentication;
using WebApi.Models;
using WebApi.Repositories;
using WebApi.Services;

namespace WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseMySql(builder.Configuration.GetConnectionString(
                "Development"
            ), ServerVersion.AutoDetect(
                builder.Configuration.GetConnectionString("Development")
            ));
        });
        ConfigureDependencyInjectionContainer(builder.Services);
        builder.Services.AddLogging();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Movie Booking System API v1",
                Description = "An ASP.NET Core Web API for movie booking system.",
                Contact = new OpenApiContact
                {
                    Name = "Sakthi Santhosh",
                    Url = new Uri("https://sakthisanthosh.in")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/license/MIT")
                }
            });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "basic",
                In = ParameterLocation.Header,
                Description = "Basic Authentication"
            };

            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                In = ParameterLocation.Header,
                Description = "JWT Authentication",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            options.AddSecurityDefinition("Basic", securityScheme);
            options.AddSecurityDefinition("Bearer", jwtSecurityScheme);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Basic"
                        }
                    },
                    new string[] {}
                },
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:SecretKey"]
                    ?? throw new ArgumentNullException("JWT secret key cannot be empty."))
                )
            };
        }).AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(
            "Basic", options => { });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            options.AddPolicy("User", policy => policy.RequireRole("User"));

        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(context =>
            {
                context.SwaggerEndpoint("v1/swagger.json", "Movie Booking System API v1");
            });
        }

        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }

    public static void ConfigureDependencyInjectionContainer(
        IServiceCollection serviceBuilder)
    {
        serviceBuilder.AddScoped<IBaseRepository<Cast>, CastRepository>();
        serviceBuilder.AddScoped<IBaseRepository<Facility>, FacilityRepository>();
        serviceBuilder.AddScoped<IBaseRepository<Movie>, MovieRepository>();
        serviceBuilder.AddScoped<IBaseRepository<MovieShow>, MovieShowRepository>();
        serviceBuilder.AddScoped<IBaseRepository<Payment>, PaymentRepository>();
        serviceBuilder.AddScoped<IBaseRepository<Promo>, PromoRepository>();
        serviceBuilder.AddScoped<IBaseRepository<Review>, ReviewRepository>();
        serviceBuilder.AddScoped<IBaseRepository<SeatType>, SeatTypeRepository>();
        serviceBuilder.AddScoped<IBaseRepository<Theater>, TheaterRepository>();
        serviceBuilder.AddScoped<IBaseRepository<Ticket>, TicketRepository>();
        serviceBuilder.AddScoped<IBaseRepository<Screen>, ScreenRepository>();
        serviceBuilder.AddScoped<ISeatRepository, SeatRepository>();
        serviceBuilder.AddScoped<IBaseRepository<User>, UserRepository>();

        serviceBuilder.AddScoped<ICastService, CastService>();
        serviceBuilder.AddScoped<IFacilityService, FacilityService>();
        serviceBuilder.AddScoped<IMovieService, MovieService>();
        serviceBuilder.AddScoped<IMovieShowService, MovieShowService>();
        serviceBuilder.AddScoped<IPaymentService, PaymentService>();
        serviceBuilder.AddScoped<IPromoService, PromoService>();
        serviceBuilder.AddScoped<IReviewService, ReviewService>();
        serviceBuilder.AddScoped<ISeatTypeService, SeatTypeService>();
        serviceBuilder.AddScoped<ITheaterService, TheaterService>();
        serviceBuilder.AddScoped<ITicketService, TicketService>();
        serviceBuilder.AddScoped<IScreenService, ScreenService>();
        serviceBuilder.AddScoped<ISeatService, SeatService>();
        serviceBuilder.AddScoped<IUserService, UserService>();

        serviceBuilder.AddScoped<IJwtService, JwtService>();

        serviceBuilder.AddSingleton<ICryptographyService, CryptographyService>();
        serviceBuilder.AddSingleton<IEmailService, EmailService>();
    }
}
