using MediatR;

namespace Financas.Application.Commands.Transacoes;

public record RemoverTransacaoCommand(Guid Id, Guid UsuarioId) : IRequest;