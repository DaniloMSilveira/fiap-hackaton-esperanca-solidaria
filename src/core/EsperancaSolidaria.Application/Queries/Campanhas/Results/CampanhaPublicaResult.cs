namespace EsperancaSolidaria.Application.Queries.Campanhas.Results;

public record CampanhaPublicaResult(
    Guid Id,
    string Titulo,
    string Descricao,
    decimal MetaFinanceira,
    decimal ValorArrecadado,
    DateTime DataInicio,
    DateTime DataFim
);
