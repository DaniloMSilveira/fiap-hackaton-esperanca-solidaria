using System.ComponentModel;

namespace EsperancaSolidaria.Domain.Enums;

public enum EPerfilAcesso
{
    [Description("Doador")]
    Doador = 1,
    
    [Description("Gestor ONG")]
    GestorONG = 2
}