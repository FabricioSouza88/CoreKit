using CoreKit.DataFilter.Models;

public interface IFilterProcessor<TResult>
{
    /// <summary>
    /// Builds a WHERE-like clause from a filter group.
    /// </summary>
    /// <param name="filter">The filter group containing rules and logic.</param>
    /// <returns>SQL WHERE clause or equivalent syntax for the target system.</returns>
    TResult BuildWhereClause(FilterGroup filter);

    /// <summary>
    /// Builds the full query components (filter, sort, pagination).
    /// </summary>
    /// <param name="request">A filter request with filters, sort rules and pagination.</param>
    /// <returns>The resulting query string (e.g., for SQL: WHERE ... ORDER BY ... LIMIT ...).</returns>
    TResult BuildQuery(FilterRequest request);
}