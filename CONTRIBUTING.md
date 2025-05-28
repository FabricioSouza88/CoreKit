# Contributing to CoreKit

Thank you for your interest in contributing to **CoreKit**! Your help is essential to make this library better for everyone.

CoreKit is designed to provide a clean and extensible foundation for building dynamic filtering systems in .NET. It supports multiple providers, including LINQ, SQL, and MongoDB.

---

## 📦 Repository Structure

- `src/CoreKit.DataFilter`: Core models and abstractions (filter request, operators, processors).
- `src/CoreKit.DataFilter.Linq`: LINQ-based implementation for Entity Framework / IQueryable.
- `src/CoreKit.DataFilter.Sql`: SQL-based filter string generator (supports SQL Server, PostgreSQL, MySQL).
- `src/CoreKit.DataFilter.Mongo`: MongoDB-specific filter builder.
- `tests/CoreKit.DataFilter.Linq.Tests`: Unit test projects for "CoreKit.DataFilter.Linq".
- `tests/CoreKit.DataFilter.Sql.Tests`: Unit test projects for "CoreKit.DataFilter.Sql".
- `tests/CoreKit.DataFilter.Mongo.Tests`: Unit test projects for "CoreKit.DataFilter.Mongo".

---

## 🚀 Getting Started

1. Fork the repository
2. Clone it locally:
   ```bash
   git clone https://github.com/FabricioSouza88/CoreKit.git
   cd CoreKit
   ```

3. Open the solution:
   ```bash
   src/CoreKit.sln
   ```

4. Build all projects and run the tests:
   ```bash
   dotnet build
   dotnet test
   ```

---

## 🧪 Testing

Please ensure you **add or update unit tests** related to your changes. We use `xUnit` for testing.

For LINQ/EF and Mongo, use **in-memory providers** or mocking where possible.

> Example test projects:
> - `CoreKit.DataFilter.Linq.Tests`
> - `CoreKit.DataFilter.Sql.Tests`
> - `CoreKit.DataFilter.Mongo.Tests`

---

## ✍️ Code Style

- Use **English** for all code, comments, and documentation.
- Use **`internal`** access modifier unless a class/method is meant for public consumption.
- Keep code aligned with **.NET 8 minimal conventions**.
- Avoid unnecessary dependencies. Use lightweight abstractions.
- Keep in mind clean-code and SOLID
---

## 💡 Feature Suggestions

If you’re planning a larger contribution (like a new processor), please open an **Issue** or **Discussion** first to align on design.

We encourage contributions such as:
- Support for other databases
- Performance improvements
- Extensions for sorting and pagination
- Bug fixes and compatibility

---

## 📦 Commit Messages

Use conventional commit messages:

```
feat: add IN support for PostgreSQL
fix: resolve null handling in SqlServerFilterProcessor
test: add pagination tests for LINQ processor
```

---

## 📄 License

By contributing, you agree that your contributions will be licensed under the MIT License, as stated in the root `LICENSE` file.

---

Thank you again for contributing to CoreKit! Let’s build great tooling for .NET developers together. 💙