using MediatR;
using TournamentApp.Application.DTOs;

namespace TournamentApp.Application.Queries;

public class GetPlayersQuery : IRequest<List<PlayerDto>>
{
}

