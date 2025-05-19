# 📦 CoreKit

**CoreKit** é um conjunto de bibliotecas utilitárias para desenvolvedores .NET que desejam turbinar a produtividade no desenvolvimento de **APIs e aplicações backend**, com foco em **filtros dinâmicos**, **manipulação de arquivos (CSV/Excel)** e **eventos internos**.

> Desenvolvido por [Fabricio Souza](https://github.com/seuusuario), o CoreKit é modular, extensível e baseado em boas práticas como Clean Code, reutilização e testes automatizados.

---

## 🧩 Pacotes disponíveis

| Pacote NuGet | Descrição |
|--------------|-----------|
| **CoreKit.DataFilter**         | Núcleo para geração de filtros dinâmicos com múltiplos operadores e suporte a aninhamento. |
| **CoreKit.DataFilter.Linq**    | Integração com LINQ para aplicar filtros em `IQueryable`, ideal para Entity Framework. |
| **CoreKit.DataFilter.Sql**     | Geração de cláusulas SQL dinâmicas com base em filtros estruturados. |
| **CoreKit.DataFilter.Mongo**   | Geração de filtros compatíveis com `FilterDefinition` do MongoDB. |
| **CoreKit.EventHandler**       | Infraestrutura leve para publicação e tratamento de eventos em aplicações. |
| **CoreKit.CsvReader**          | Leitura de arquivos CSV com mapeamento direto para objetos DTO. |
| **CoreKit.CsvWriter**          | Escrita de coleções de objetos em arquivos CSV. |
| **CoreKit.ExcelWriter**        | Escrita de planilhas Excel (`.xlsx`) a partir de listas de objetos, com suporte a formatação. |

---

## 🚀 Instalação

Você pode instalar os pacotes desejados via NuGet CLI:

```bash
dotnet add package CoreKit.DataFilter
dotnet add package CoreKit.ExcelWriter
```

Ou via referência direta no `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="CoreKit.DataFilter" Version="1.0.0" />
  <PackageReference Include="CoreKit.ExcelWriter" Version="1.0.0" />
</ItemGroup>
```

---

## 💡 Exemplo rápido

### 🔎 Filtro dinâmico com LINQ

```csharp
var request = new SimpleQueryRequest
{
    Filters = new List<SimpleFilter>
    {
        new SimpleFilter("name", FilterOperatorEnum.Contains, "john")
    }
};

var processor = new GenericLinqFilterProcessor<User>();
var filtered = processor.ApplyFilters(users.AsQueryable(), request);
```

### 📤 Escrita de CSV

```csharp
var csvWriter = new CsvWriter<User>();
var content = csvWriter.Write(usersList);
File.WriteAllText("users.csv", content);
```

---

## 📚 Exemplos

Exemplos completos podem ser encontrados na pasta [`/examples`](./examples) (em breve). Cada pacote também possui seu próprio `README.md` com instruções específicas.

---

## 🛠 Requisitos

- .NET 6 ou superior (recomendado: .NET 8)
- Para MongoDB: `MongoDB.Driver`
- Para Excel: `ClosedXML`

---

## 🤝 Contribuindo

Pull requests são bem-vindos! Para contribuir:

1. Fork este repositório
2. Crie uma branch com a sua feature (`git checkout -b feature/nome-da-feature`)
3. Commit suas alterações (`git commit -m 'feat: descrição'`)
4. Push para a branch (`git push origin feature/nome-da-feature`)
5. Abra um Pull Request 🚀

---

## 📦 Publicação no NuGet

Cada projeto do CoreKit é empacotado e publicado no [NuGet.org](https://www.nuget.org/) com versionamento independente. O processo de build e publish é automatizado via GitHub Actions.

---

## 📄 Licença

Este projeto está licenciado sob os termos da [MIT License](./LICENSE).

---

Desenvolvido com ❤️ por [Fabricio Souza](https://github.com/seuusuario)
