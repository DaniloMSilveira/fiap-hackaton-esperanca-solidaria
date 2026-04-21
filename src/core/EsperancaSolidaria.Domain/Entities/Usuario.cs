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
        DataCriacao = DateTime.Now;
        UsuarioCriacao = usuario;

        NomeCompleto = nomeCompleto;
        Email = email;
        Cpf = cpf;
        SenhaCriptografada = senhaCriptografada;
        PerfilAcesso = perfilAcesso;
        Ativo = true;
    }

    public void AlterarSenha(string senhaCriptografada, string usuario)
    {
        SenhaCriptografada = senhaCriptografada;
        DataAtualizacao = DateTime.Now;
        UsuarioAtualizacao = usuario;
    }

    public void AlterarDados(string nomeCompleto, Cpf cpf, EPerfilAcesso perfilAcesso, string usuario)
    {
        NomeCompleto = nomeCompleto;
        Cpf = cpf ?? throw new ArgumentNullException(nameof(cpf));
        PerfilAcesso = perfilAcesso;
        DataAtualizacao = DateTime.Now;
        UsuarioAtualizacao = usuario;
    }

    public void Inativar(string usuario)
    {
        Ativo = false;
        DataAtualizacao = DateTime.Now;
        UsuarioAtualizacao = usuario;
    }

    public void Ativar(string usuario)
    {
        Ativo = true;
        DataAtualizacao = DateTime.Now;
        UsuarioAtualizacao = usuario;
    }
}