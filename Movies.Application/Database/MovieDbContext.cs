using Microsoft.EntityFrameworkCore;
using Movies.Application.Models;

namespace Movies.Application.database
{

    using Entities;

    public class MovieDbContext : DbContext
    {

        public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; init; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Rating> Ratings { get; init; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>()
                .Ignore(m => m.Rating)
                .HasMany(m => m.Ratings)
                // .WithOne(r => r.Movie) // If the movie has a navigation property to the rating
                .WithOne(rating => rating.Movie) // If the movie doesn't has a navigation property to the rating
                .HasForeignKey(r => r.MovieId) // Assuming Rating has a foreign key property named MovieId
                // .HasForeignKey("MovieId") // Shadow property
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Genre>()
                .HasIndex(g => g.Name)
                .IsUnique();

            // modelBuilder.Entity<Movie>()
            //     .HasMany(m => m.MovieRatings)
            //     .WithOne(r => r.Movie)
            //     .HasForeignKey(r => r.MovieId)
            //     .OnDelete(DeleteBehavior.Cascade);
            //
            // modelBuilder.Entity<Genre>()
            //     .HasIndex(g => g.Name)
            //     .IsUnique();
            //
            // modelBuilder.Entity<MovieGenre>()
            //     .HasKey(mg => new { mg.MovieId, mg.GenreId });
            //
            // modelBuilder.Entity<MovieGenre>()
            //     .HasOne(mg => mg.Movie)
            //     .WithMany(m => m.MovieGenres)
            //     .HasForeignKey(mg => mg.MovieId)
            //     .OnDelete(DeleteBehavior.Cascade);
            //
            // modelBuilder.Entity<MovieGenre>()
            //     .HasOne(mg => mg.Genre)
            //     .WithMany()
            //     .HasForeignKey(mg => mg.GenreId);
        }

    }

}
