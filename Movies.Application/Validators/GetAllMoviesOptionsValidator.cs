using FluentValidation;
using Movies.Contracts.Requests;

namespace Movies.Application.Validators;

public class GetAllMoviesOptionsValidator : AbstractValidator<GetAllMoviesRequest>
{

    private static readonly string[] AcceptableSortFields =
    {
        "title", "yearofrelease"
    };

    public GetAllMoviesOptionsValidator()
    {
        RuleFor(x => x.Year)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);

        RuleFor(x => x.SortBy)
            .Must(x => x is null || AcceptableSortFields.Contains(x, StringComparer.OrdinalIgnoreCase))
            .WithMessage("You can only sort by 'title' or 'yearofrelease'");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 25)
            .WithMessage("You can get between 1 and 25 movies per page");
    }

}
