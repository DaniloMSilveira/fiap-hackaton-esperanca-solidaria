using System.ComponentModel;

namespace EsperancaSolidaria.Domain.Enums;

public enum EStatusCampanha
{
    [Description("Ativa")]
    Ativa = 1,

    [Description("Concluída")]
    Concluida = 2,

    [Description("Cancelada")]
    Cancelada = 3
}
