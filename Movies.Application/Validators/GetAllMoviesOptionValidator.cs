using FluentValidation;

namespace Movies.Application.Validators; 

public class GetAllMoviesOptionValidator  : AbstractValidator<GetAllMoviesOptions> {
    
    private static readonly string [] ValidSortFields = {"title", "yearofrelease"};
    
    public GetAllMoviesOptionValidator() {
        // can't search for something in the future
        RuleFor(x => x.Year)
            .LessThanOrEqualTo(DateTime.Now.Year)
            .When(x => x.Year.HasValue);
        
        RuleFor(x => x.SortField)
            .Must(x => x == null || ValidSortFields.Contains(x, StringComparer.Ordinal))
            .WithMessage($"Sort field must be one of {string.Join(", ", ValidSortFields)}");
    }
    
}