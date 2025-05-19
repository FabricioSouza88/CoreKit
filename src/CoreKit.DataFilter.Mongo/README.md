# CoreKit.DataFilter.Mongo

This library provides a dynamic filter processor for MongoDB, allowing you to transform a `FilterRequest` into a MongoDB-compatible query using the MongoDB .NET Driver (`IMongoCollection<T>`).

It supports:

- Dynamic filters (`AND` / `OR`, including nested groups)
- Mongo-specific operators like `Regex` for string search
- Pagination with `Skip` and `Limit`
- Sorting with multiple fields

---

## ✅ Components

### `MongoFilterProcessor<T>`

Implements the interface `IMongoFilterProcessor<T>`, providing methods to:

- Build a `FilterDefinition<T>` from a `FilterRequest`
- Apply sorting
- Handle pagination
- Combine everything using `ApplyAll(...)`

---

## ✨ Example Usage

```csharp
[HttpPost("search")]
public IActionResult Search([FromBody] FilterRequest request)
{
    var processor = new MongoFilterProcessor<Product>();
    var result = processor
        .ApplyAll(_mongo.Products, request)
        .ToList();

    return Ok(result);
}
```

---

## 🧾 Example FilterRequest JSON (POST)

```json
{
  "filter": {
    "logic": "and",
    "rules": [
      { "field": "status", "operator": "Equals", "value": "active" },
      { "field": "name", "operator": "Contains", "value": "book" }
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

## 🧠 Supported Operators

| Operator | Mongo Mapping |
|----------|----------------|
| Equals / NotEquals | `.Eq() / .Ne()` |
| Contains | `.Regex(..., "i")` |
| StartsWith / EndsWith | `.Regex("^value")` / `.Regex("value$")` |
| Greater / Less / >= / <= | `.Gt(), .Lt(), .Gte(), .Lte()` |
| In | `.In()` with comma-separated values |
| Null / NotNull | `.Eq(null)` / `.Ne(null)` |

---

## 📦 Dependencies

- MongoDB.Driver (v2.19+ recommended)

```bash
dotnet add package MongoDB.Driver
```

---

## 📘 Notes

- The processor uses `BsonRegularExpression` for `Contains`, `StartsWith`, and `EndsWith` (case-insensitive).
- Pagination is done with `.Skip()` and `.Limit()`.
- Sorting supports multiple fields combined using `Sort.Combine()`.
