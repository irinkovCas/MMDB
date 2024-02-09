using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.database;
using Movies.Application.Repositorires;
using Movies.Application.Services;

namespace Movies.Application {

    public static class ApplicationServiceCollectionExtensions {

        public static IServiceCollection AddApplication(this IServiceCollection services) {
            services.AddSingleton<IMovieService, MovieService>();
            services.AddSingleton<IMovieRepository, MovieRepository>(); // Singleton because of validation
            services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);

            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString) {

            services.AddDbContext<MovieDbContext>(options => options.UseNpgsql(connectionString), ServiceLifetime.Singleton);

            return services;
        }

    }

}
