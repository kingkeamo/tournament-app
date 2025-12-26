using FluentValidation.Results;

namespace TournamentApp.Application.Common.Responses;

public class ValidatedResponse
{
    public ValidatedResponse()
    {
        ValidationErrors = new List<ValidationFailure>();
    }

    public bool IsSuccess => !ValidationErrors.Any() && string.IsNullOrEmpty(ErrorMessage);
    public bool IsFailure => !IsSuccess;

    public IList<ValidationFailure> ValidationErrors { get; set; } 
    public string ErrorMessage { get; set; } = String.Empty;
}



