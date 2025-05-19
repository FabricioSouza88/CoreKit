using CoreKit.DataFilter.Models.Enums;

namespace CoreKit.DataFilter.Models;

public class FilterRule
{
    public string Field { get; set; } = string.Empty;
    public FilterOperatorEnum Operator { get; set; }

    /// <summary>
    /// Valor como string (serializado). Pode ser convertido posteriormente por tipo de dado.
    /// Ex: "2024-01-01", "true", "42", "[1,2,3]"
    /// </summary>
    public string? Value { get; set; }
}
