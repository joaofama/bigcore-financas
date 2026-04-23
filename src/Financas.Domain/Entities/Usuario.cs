namespace Financas.Domain.Entities;

public class Usuario
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public decimal SaldoCadastro { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataAlteracao { get; private set; }

    protected Usuario() { }

    public Usuario(string nome, string email, string senhaHash, decimal saldoCadastro)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Email = email.ToLower().Trim();
        SenhaHash = senhaHash;
        SaldoCadastro = saldoCadastro;
        Ativo = true;
        DataCriacao = DateTime.UtcNow;
    }

    public void AtualizarPerfil(string nome)
    {
        Nome = nome;
        DataAlteracao = DateTime.UtcNow;
    }

    public void AtualizarSenha(string novoSenhaHash)
    {
        SenhaHash = novoSenhaHash;
        DataAlteracao = DateTime.UtcNow;
    }

    public void Inativar()
    {
        Ativo = false;
        DataAlteracao = DateTime.UtcNow;
    }
}