using EsperancaSolidaria.BuildingBlocks.Domain;
using EsperancaSolidaria.Domain.Enums;
using EsperancaSolidaria.Domain.ValueObjects;

namespace EsperancaSolidaria.Domain.Entities;

public class Usuario : Entity, IAggregateRoot
{
    public string NomeCompleto { get; private set; }
    public Email Email { get; private set; }
    public Cpf Cpf { get; private set; }
    public string SenhaCriptografada { get; private set; }
    public EPerfilAcesso PerfilAcesso { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public string UsuarioCriacao { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }
    public string? UsuarioAtualizacao { get; private set; }

    protected Usuario() { }

    public Usuario(string nomeCompleto, Email email, Cpf cpf, string senhaCriptografada, EPerfilAcesso perfilAcesso, string usuario)
    {
        if (string.IsNullOrWhiteSpace(nomeCompleto))
            throw new ArgumentException("NomeCompleto é obrigatório");

        DataCriacao = DateTime.Now;
        UsuarioCriacao = usuario;

        NomeCompleto = nomeCompleto;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Cpf = cpf ?? throw new ArgumentNullException(nameof(cpf));
        SenhaCriptografada = senhaCriptografada ?? throw new ArgumentNullException(nameof(senhaCriptografada));
        PerfilAcesso = perfilAcesso;
        Ativo = true;
    }

    public void AlterarSenha(string senhaCriptografada, string usuario)
    {
        if (string.IsNullOrWhiteSpace(senhaCriptografada))
            throw new ArgumentException("Senha é obrigatória");

        SenhaCriptografada = senhaCriptografada;
        DataAtualizacao = DateTime.Now;
        UsuarioAtualizacao = usuario;
    }

    public void Inativar(string usuario)
    {
        Ativo = false;
        DataAtualizacao = DateTime.Now;
        UsuarioAtualizacao = usuario;
    }
}