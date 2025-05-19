# CoreKit.DataFilter

This is the core library responsible for modeling dynamic filtering logic for APIs, services, or repositories. It contains shared models and enums that represent filters, rules, logic groups, and sorting and pagination requests.

## 📦 Package Contents

- `FilterRequest`: represents a full filter request including filtering, sorting, and pagination.
- `FilterGroup`: supports logical grouping of multiple filter rules (`AND` / `OR`).
- `FilterRule`: a single filtering condition (field, operator, value).
- `SortRule`: ordering rule for a single field.
- `Pagination`: page and pageSize configuration.
- `FilterOperatorEnum`: available filtering operations like `Equals`, `Contains`, `In`, `GreaterThan`, etc.

## ✅ Supported Operators

| Operator | Description |
|----------|-------------|
| Equals / NotEquals | Equality check |
| Contains / StartsWith / EndsWith | String operations |
| GreaterThan / LessThan | Numeric/date comparison |
| In | Multiple values |
| Null / NotNull | Null checks |

## 🧪 Example

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
    "pageSize": 10
  }
}
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
