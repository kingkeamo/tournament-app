using MediatR;
using TournamentApp.Application.DTOs;

namespace TournamentApp.Application.Queries;

public class GetTournamentListQuery : IRequest<List<TournamentDto>>
{
}

