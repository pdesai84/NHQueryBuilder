# NHQueryBuilder

**NHQueryBuilder** — a fluent, strongly-typed query builder for NHibernate.

NHQueryBuilder simplifies dynamic, complex querying in NHibernate using a readable, composable, lambda-based DSL. It eliminates aliasing and fragile string paths while supporting features like `IsIn`, `Between`, `OrderBy`, `Fetch`, `OR` groups, `Skip`/`Take`, and more.

---

## Features

- Fluent, strongly-typed condition builder (`Equal`, `IsIn`, `Between`, `IsNull`, `IsNotNull`, `Like`, etc.)
- Automatic handling of nested joins and navigation paths
- Composable OR / AND groups
- Pagination: `Skip` and `Take`
- `OrderBy` ascending/descending
- Integration with NHibernate `ISession` via extension methods (`GetList`, `Get`, `GetCount`)
- ORM-agnostic core (future adapter support planned)

---

## Install

```bash
dotnet add package NHQueryBuilder