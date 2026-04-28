using Financas.Application.Responses.Categorias;
using MediatR;

namespace Financas.Application.Commands.Categorias;

public record CriarCategoriaCommand(
    Guid UsuarioId,
    string Nome,
    string Tipo, // "R" ou "D"
    string Icone,
    Guid? CategoriaPaiId // Se null = Pai, se preenchido = Filha
) : IRequest<CategoriaResponse>;