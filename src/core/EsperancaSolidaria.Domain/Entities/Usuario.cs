using EsperancaSolidaria.Domain.Enums;
using EsperancaSolidaria.Domain.Interfaces;
using EsperancaSolidaria.Domain.ValueObjects;

namespace EsperancaSolidaria.Domain.Entities;

public class Usuario : IEntity, IAggregateRoot
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public Email Email { get; private set; }
    public Cpf Cpf { get; private set; }
    public string SenhaCriptografada { get; private set; }
    public UserRole Perfil { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }

    protected Usuario() { }

    public Usuario(string nome, Email email, Cpf cpf, string senhaCriptografada, UserRole perfil)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório");

        Id = Guid.NewGuid();
        DataCriacao = DateTime.UtcNow;

        Nome = nome;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Cpf = cpf ?? throw new ArgumentNullException(nameof(cpf));
        SenhaCriptografada = senhaCriptografada ?? throw new ArgumentNullException(nameof(senhaCriptografada));
        Perfil = perfil;
        Ativo = true;
    }

    public void AlterarSenha(string senhaCriptografada)
    {
        if (string.IsNullOrWhiteSpace(senhaCriptografada))
            throw new ArgumentException("Senha é obrigatória");

        SenhaCriptografada = senhaCriptografada;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Inativar()
    {
        Ativo = false;
        DataAtualizacao = DateTime.UtcNow;
    }
}