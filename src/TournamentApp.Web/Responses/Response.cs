using FluentValidation.Results;

namespace TournamentApp.Web.Responses;

public class Response
{
    public Response()
    {
        ValidationErrors = new List<ValidationFailure>();
    }

    public bool IsSuccess => !ValidationErrors.Any() && string.IsNullOrEmpty(ErrorMessage);
    public bool IsFailure => !IsSuccess;
    public string ErrorMessage { get; set; } = string.Empty;
    public IList<ValidationFailure> ValidationErrors { get; set; }
}

public class DataResponse<T> : Response
{
    public T Data { get; set; } = default!;
}

public class CreateResponse : Response
{
    public Guid NewId { get; set; }
}

