using MediatR;
using TournamentApp.Shared;

namespace TournamentApp.Application.Queries;

public class GetTournamentListQuery : IRequest<List<TournamentDto>>
{
}

