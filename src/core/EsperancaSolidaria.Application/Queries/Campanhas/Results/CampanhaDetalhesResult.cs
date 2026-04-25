using EsperancaSolidaria.Domain.Enums;

namespace EsperancaSolidaria.Application.Queries.Campanhas.Results;

public record CampanhaDetalhesResult(
    Guid Id,
    string Titulo,
    string Descricao,
    DateTime DataInicio,
    DateTime DataFim,
    decimal MetaFinanceira,
    decimal ValorArrecadado,
    EStatusCampanha StatusId,
    string StatusDescricao,
    DateTime DataCriacao,
    string UsuarioCriacao
);