using Movies.Application.Models;
using Movies.Contracts.Entities;

namespace Movies.Api.Mapping;

public static class Mapping {

    public static MovieDto ToDto(this Movie movie) => new MovieDto {
        Title = movie.Title,
    };

}
