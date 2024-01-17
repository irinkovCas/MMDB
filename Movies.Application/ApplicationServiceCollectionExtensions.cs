﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.database;
using Movies.Application.Repositorires;
using Movies.Application.Services;

namespace Movies.Application {

    public static class ApplicationServiceCollectionExtensions {

        public static IServiceCollection AddApplication(this IServiceCollection services) {
            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<IMovieRepository, MovieRepository>();

            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString) {

            services.AddSingleton<IDbConnectionFactory>(new DbConnectionFactory(connectionString));
            services.AddDbContext<MovieDbContext>(options => options.UseNpgsql(connectionString));

            return services;
        }

    }

}