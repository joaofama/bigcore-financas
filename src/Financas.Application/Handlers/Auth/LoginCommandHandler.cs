using Financas.Application.Requests.Auth;
using Financas.Application.Responses.Auth;
using Financas.Domain.Interfaces.Repositories;
using Financas.Domain.Interfaces.Services;
using MediatR;

namespace Financas.Application.Handlers.Auth;

public class LoginCommandHandler : IRequestHandler<LoginRequest, LoginResponse>
{
    private readonly ITokenService _tokenService;
    private readonly IUsuarioRepository _usuarioRepository;

    public LoginCommandHandler(ITokenService tokenService, IUsuarioRepository usuarioRepository)
    {
        _tokenService = tokenService;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        // 1. Busca direta pela combinação de e-mail e senha
        var usuario = await _usuarioRepository.ObterPorCredenciaisAsync(request.Email, request.Senha);

        // 2. Se for nulo, significa que ou o e-mail não existe ou a senha está errada
        if (usuario == null)
        {
            throw new UnauthorizedAccessException("Credenciais inválidas.");
        }

        // 3. Gerar o token com os dados que vieram do documento encontrado
        var token = _tokenService.GerarToken(usuario.Id, usuario.Email, usuario.Nome);

        return new LoginResponse(
            Token: token,
            Nome: usuario.Nome,
            Email: usuario.Email,
            SaldoInicial: 0m,
            DataCadastro: usuario.DataCriacao
        );
    }
}