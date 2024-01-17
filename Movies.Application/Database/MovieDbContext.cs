using Microsoft.EntityFrameworkCore;
using Movies.Application.Models;

namespace Movies.Application.database {

    public class MovieDbContext : DbContext {

        public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options) {
        }

        public DbSet<Movie> Movies { get; set; }
        // Add other DbSet properties for other tables/entities as needed

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
        }

    }

}
