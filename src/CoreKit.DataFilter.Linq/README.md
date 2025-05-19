# CoreKit.DataFilter.Linq

This library allows you to apply dynamic filtering (`FilterRequest`) to `IQueryable<T>` in LINQ/EF Core using expression trees. It supports:

- Filtering with dynamic conditions
- Sorting with Dynamic LINQ
- Pagination using Skip/Take

## 🔧 Key Component

### `GenericLinqFilterProcessor<T>`

Builds a `Func<T, bool>` expression tree based on a `FilterRequest` and applies it to a LINQ query.

## ✅ Usage

### With `ApplyFilter(...)`

```csharp
var processor = new GenericLinqFilterProcessor<Product>();
var query = processor.ApplyFilter(_dbContext.Products, request);
```

### With extension method:

```csharp
var result = _dbContext.Products
    .AsQueryable()
    .ApplyFilterRequest(request)
    .ToList();
```

> Requires reference to `CoreKit.DataFilter.Linq.Extensions`

## 📦 Supported Features

- All `FilterOperatorEnum` operators
- Nested groups with `AND` / `OR`
- IN, NULL, NOT NULL support
- Sorting via [System.Linq.Dynamic.Core](https://www.nuget.org/packages/System.Linq.Dynamic.Core)

## 📦 NuGet Requirements

```bash
dotnet add package System.Linq.Dynamic.Core
```

---

## 🎯 How to receive filters in your Controller

You can support both `GET` and `POST` methods in your controller for filtering, but they have different behaviors:

### ✅ Option 1: POST (Recommended for complex filters)

```csharp
[HttpPost("search")]
public IActionResult Search([FromBody] FilterRequest request)
{
    var result = _dbContext.Products
        .AsQueryable()
        .ApplyFilterRequest(request)
        .ToList();

    return Ok(result);
}
```

### ⚠️ Option 2: GET (Limited support)

```csharp
[HttpGet("search")]
public IActionResult Search([FromQuery] SimpleQueryRequest query)
{
    var request = query.ToFilterRequest();

    var result = _dbContext.Products
        .AsQueryable()
        .ApplyFilterRequest(request)
        .ToList();

    return Ok(result);
}
```

### ⚠️ Limitations of GET filtering

| Limitation | Description |
|------------|-------------|
| No body | GET requests cannot use `[FromBody]`, so the entire filter must fit in the query string |
| No nested groups | Only flat `FilterRule` lists are supported (no AND/OR groups) |
| Limited operators | Only basic `eq`, `neq`, `gt`, `lt`, `in`, `null`, etc. |
| Limited characters | Query strings are size-limited and must be URL-encoded |

---

## 🧮 How to use SimpleQueryRequest in a GET

### 📌 Query string parameters

| Parameter     | Description |
|---------------|-------------|
| `filterQuery` | A semicolon-separated list of simple filters |
| `sort`        | Field name to sort by |
| `sortDir`     | Sorting direction: `asc` or `desc` |
| `page`        | Page number (starting from 1) |
| `pageSize`    | Number of items per page |

---

### 🧪 Example GET request

```
GET /api/products/search
  ?filterQuery=status:eq:active;price:gt:100
  &sort=createdAt
  &sortDir=desc
  &page=2
  &pageSize=10
```

---

### 🧾 Syntax for `filterQuery`

Each filter is composed of:

```
field:operator:value
```

- Use `;` to separate multiple rules.
- To use OR logic, prefix each rule with `or:`.

---

### ✅ Supported operators in `filterQuery`

| Operator | Syntax | Example |
|----------|--------|---------|
| Equals | `eq` | `status:eq:active` |
| Not Equals | `neq` | `status:neq:disabled` |
| Greater Than | `gt` | `price:gt:100` |
| Less Than | `lt` | `price:lt:500` |
| Greater Than or Equal | `gte` | `stock:gte:10` |
| Less Than or Equal | `lte` | `stock:lte:5` |
| Contains | `contains` | `name:contains:book` |
| Starts With | `starts` | `name:starts:Pro` |
| Ends With | `ends` | `name:ends:X` |
| In List | `in` | `category:in:Books,Electronics` |
| Is Null | `null` | `deletedAt:null` |
| Is Not Null | `notnull` | `deletedAt:notnull` |

---

### 🧠 Important Notes

- Values in `in` must be comma-separated and **not quoted**.
- Use `or:` prefix for logical OR:

  ```
  or:category:eq:Books;or:category:eq:Stationery
  ```

- Nested groups and complex logical combinations (`AND` + `OR`) are not supported in GET.

---

### 🔧 Controller example

```csharp
[HttpGet("search")]
public IActionResult Search([FromQuery] SimpleQueryRequest query)
{
    var request = query.ToFilterRequest();
    var result = _dbContext.Products
        .AsQueryable()
        .ApplyFilterRequest(request)
        .ToList();

    return Ok(result);
}
```

---

## 📬 How to use FilterRequest in a POST

Use POST when you need full filtering capabilities including:
- Grouped filters (`AND` / `OR`)
- Operators like `IN`, `NULL`, `NOT NULL`
- Large payloads not suitable for query strings

---

### 📦 Example JSON body

```json
{
  "filter": {
    "logic": "and",
    "rules": [
      { "field": "status", "operator": "Equals", "value": "active" },
      { "field": "price", "operator": "GreaterThan", "value": "100" }
    ],
    "groups": [
      {
        "logic": "or",
        "rules": [
          { "field": "category", "operator": "Equals", "value": "Books" },
          { "field": "category", "operator": "Equals", "value": "Stationery" }
        ]
      }
    ]
  },
  "sort": [
    { "field": "createdAt", "direction": "desc" }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20
  }
}
```

---

### 🔧 Controller example

```csharp
[HttpPost("search")]
public IActionResult Search([FromBody] FilterRequest request)
{
    var result = _dbContext.Products
        .AsQueryable()
        .ApplyFilterRequest(request)
        .ToList();

    return Ok(result);
}
```
