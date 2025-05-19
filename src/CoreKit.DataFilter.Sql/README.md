# CoreKit.DataFilter.Sql

This library provides SQL-compatible processors that transform a `FilterRequest` into SQL syntax: `WHERE`, `ORDER BY`, and pagination clauses. It includes a generic ANSI-compatible processor and a specialized one for SQL Server.

## 🔧 Processors

### `GenericSqlFilterProcessor`
- Compatible with PostgreSQL, MySQL, SQLite
- Uses `LIMIT {pageSize} OFFSET {skip}` for pagination

### `SqlServerFilterProcessor`
- Inherits from `GenericSqlFilterProcessor`
- Overrides pagination to use `OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY`

## ✅ Usage

```csharp
var processor = new GenericSqlFilterProcessor();
string sql = processor.BuildQuery(filterRequest);
```

This will return a SQL string like:

```sql
WHERE status = 'active' AND price > '50'
ORDER BY createdAt DESC
LIMIT 10 OFFSET 20
```

### Example for SQL Server:

```csharp
var processor = new SqlServerFilterProcessor();
string sql = processor.BuildQuery(request);
```

Output:

```sql
WHERE ...
ORDER BY createdAt DESC
OFFSET 20 ROWS FETCH NEXT 10 ROWS ONLY
```

## 🧪 Integrating with raw SQL or Dapper

```csharp
string sql = processor.BuildQuery(request);
var items = dbConnection.Query<Product>($"SELECT * FROM Products {sql}");
```
