using MediatR;
using TournamentApp.Shared;

namespace TournamentApp.Application.Queries;

public class GetPlayersQuery : IRequest<List<PlayerDto>>
{
}

