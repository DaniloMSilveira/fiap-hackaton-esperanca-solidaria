using EsperancaSolidaria.Application.DTO.Inputs;
using EsperancaSolidaria.Application.DTO.Outputs;
using EsperancaSolidaria.Domain.ValueObjects;
using EsperancaSolidaria.Domain.Entities;
using EsperancaSolidaria.Domain.Enums;
using EsperancaSolidaria.Domain.Interfaces.Repositories;
using EsperancaSolidaria.Domain.Interfaces;
using EsperancaSolidaria.Application.Services.Interfaces;
using EsperancaSolidaria.BuildingBlocks.Persistence;

namespace EsperancaSolidaria.Application.Services;

using CriarUsuarioResult = BaseOutput<UsuarioOutput>;

public class UsuarioAppService : IUsuarioAppService
{
    private readonly IUsuarioRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UsuarioAppService(IUsuarioRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseOutput<UsuarioOutput>> CriarUsuario(CriarUsuarioInput input)
    {
        if (!input.IsValid())
            return CriarUsuarioResult.Fail(input.ValidationResult);

        var existeUsuario = await _repository.ExisteAsync(input.Email);
        if (existeUsuario)
            return CriarUsuarioResult.Fail("Já existe um usuário cadastrado com este e-mail.");

        var email = new Email(input.Email);
        var cpf = new Cpf(input.Cpf);
        var senhaCriptografada = BCrypt.Net.BCrypt.HashPassword(input.Senha);

        var usuario = new Usuario(input.Nome, email, cpf, senhaCriptografada, UserRole.Doador);
        _repository.Adicionar(usuario);

        return CriarUsuarioResult.Ok(new UsuarioOutput(usuario.Id, usuario.Nome, usuario.Email.Value, usuario.Cpf.Value, UserRole.Doador.ToString()));
    }
}