using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsperancaSolidaria.Application.DTO.Inputs;
using EsperancaSolidaria.Application.DTO.Outputs;

namespace EsperancaSolidaria.Application.Services.Interfaces;

public interface IUsuarioAppService
{
    // Task<PaginacaoOutput<UsuarioItemListaOutput>> PesquisarUsuarios(PesquisarUsuariosQuery query);
    // Task<UsuarioOutput?> ObterPorId(Guid id);
    Task<BaseOutput<UsuarioOutput>> CriarUsuario(CriarUsuarioInput input);
    // Task<BaseOutput> Remover(Guid id);
}