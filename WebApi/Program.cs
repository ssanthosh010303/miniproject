using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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
        serviceBuilder.AddScoped<IBaseRepository<Promo>, PromoRepository>();
        serviceBuilder.AddScoped<IBaseRepository<Review>, ReviewRepository>();
        serviceBuilder.AddScoped<IBaseRepository<SeatType>, SeatTypeRepository>();
        serviceBuilder.AddScoped<IBaseRepository<Theater>, TheaterRepository>();
        serviceBuilder.AddScoped<IBaseRepository<Ticket>, TicketRepository>();
        serviceBuilder.AddScoped<ISeatRepository, SeatRepository>();

        serviceBuilder.AddScoped<ICastService, CastService>();
        serviceBuilder.AddScoped<IFacilityService, FacilityService>();
        serviceBuilder.AddScoped<IMovieService, MovieService>();
        serviceBuilder.AddScoped<IMovieShowService, MovieShowService>();
        serviceBuilder.AddScoped<IPromoService, PromoService>();
        serviceBuilder.AddScoped<IReviewService, ReviewService>();
        serviceBuilder.AddScoped<ISeatTypeService, SeatTypeService>();
        serviceBuilder.AddScoped<ITheaterService, TheaterService>();
        serviceBuilder.AddScoped<ITicketService, TicketService>();
        serviceBuilder.AddScoped<ISeatService, SeatService>();
    }
}
