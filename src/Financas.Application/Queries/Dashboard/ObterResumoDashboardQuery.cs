using MediatR;
using Financas.Application.Responses.Dashboard;

namespace Financas.Application.Queries.Dashboard;

public record ObterResumoDashboardQuery(int Mes, int Ano, Guid UsuarioId)
    : IRequest<DashboardResponse>;