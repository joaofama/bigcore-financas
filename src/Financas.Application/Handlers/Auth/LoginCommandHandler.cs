using Financas.Application.Commands.Auth; // <-- IMPORTANTE: usar o Command
using Financas.Application.Responses.Auth;
using Financas.Domain.Interfaces.Repositories;
using Financas.Domain.Interfaces.Services;
using MediatR;

namespace Financas.Application.Handlers.Auth;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly ITokenService _tokenService;
    private readonly IUsuarioRepository _usuarioRepository;

    public LoginCommandHandler(ITokenService tokenService, IUsuarioRepository usuarioRepository)
    {
        _tokenService = tokenService;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObterPorCredenciaisAsync(request.Email, request.Senha);

        if (usuario == null)
        {
            throw new UnauthorizedAccessException("Credenciais inválidas.");
        }

        var token = _tokenService.GerarToken(usuario.Id, usuario.Email, usuario.Nome);

        return new LoginResponse(
            Token: token,
            Nome: usuario.Nome,
            Email: usuario.Email,
            SaldoInicial: usuario.SaldoInicial,
            DataCadastro: usuario.DataCadastro
        );
    }
}