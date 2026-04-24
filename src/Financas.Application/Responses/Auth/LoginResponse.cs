// Financas.Application.Responses.Auth
namespace Financas.Application.Responses.Auth;

public record LoginResponse(
    string Token,
    string Nome,
    string Email,
    decimal SaldoInicial,
    DateTime DataCadastro
);