using MediatR;
using Financas.Application.Responses.Auth;

namespace Financas.Application.Requests.Auth;

public record LoginRequest(string Email, string Senha) : IRequest<LoginResponse>;