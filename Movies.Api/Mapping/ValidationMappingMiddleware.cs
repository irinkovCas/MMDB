namespace Movies.Api.Mapping;

using FluentValidation;
using Contracts.Responses;

public class ValidationMappingMiddleware
{

    private readonly RequestDelegate _next;

    public ValidationMappingMiddleware(RequestDelegate next)
    {
        this._next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await this._next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = 400;

            var validationFailureResponse = new ValidationFailureResponse
            {
                Errors = ex.Errors.Select(error => new ValidationResponse
                {
                    PropertyName = error.PropertyName, Message = error.ErrorMessage
                })
            };

            await context.Response.WriteAsJsonAsync(validationFailureResponse);
        }
    }

}
