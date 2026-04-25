using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsperancaSolidaria.Application.Queries.Doacoes.Results;

public record DoacaoResult(
    Guid Id,
    decimal Valor,
    DateTime DataDoacao,
    string NomeDoador,
    string ReferenciaPagamento
);