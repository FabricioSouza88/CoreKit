namespace CoreKit.DataFilter.Models;

public class FilterGroup
{
    /// <summary>
    /// Lógica entre as regras e/ou subgrupos: "and" ou "or"
    /// </summary>
    public string Logic { get; set; } = "and";

    /// <summary>
    /// Regras simples de filtro
    /// </summary>
    public List<FilterRule> Rules { get; set; } = new();

    /// <summary>
    /// Subgrupos de regras (aninhados)
    /// </summary>
    public List<FilterGroup> Groups { get; set; } = new();
}
