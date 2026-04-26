using Financas.Application.Responses.Auth;
using MediatR;

namespace Financas.Application.Commands.Auth;

public record LoginCommand(string Email, string Senha) : IRequest<LoginResponse>;