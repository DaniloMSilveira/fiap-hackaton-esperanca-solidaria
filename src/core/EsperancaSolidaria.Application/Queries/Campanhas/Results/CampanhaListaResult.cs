using EsperancaSolidaria.Domain.Enums;

namespace EsperancaSolidaria.Application.Queries.Campanhas.Results;

public record CampanhaListaResult(
    Guid Id,
    string Titulo,
    DateTime DataInicio,
    DateTime DataFim,
    decimal MetaFinanceira,
    decimal ValorArrecadado,
    EStatusCampanha StatusId,
    string StatusDescricao,
    DateTime DataCriacao
);
