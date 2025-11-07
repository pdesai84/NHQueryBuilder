# NHQueryBuilder

NHQueryBuilder is a fluent, strongly-typed query builder for NHibernate, designed to simplify and streamline the construction of complex queries in .NET applications. It provides a modern, expressive API for building criteria, projections, ordering, pagination, and more, all while leveraging the power of NHibernate ORM.

## Features

- **Fluent API for Query Construction**: Build queries using a chainable, strongly-typed syntax.
- **Comprehensive Condition Support**: Easily add conditions such as `Equal`, `NotEqual`, `IsIn`, `IsNotIn`, `IsNull`, `IsNotNull`, `Between`, `Like`, `NotLike`, `GreaterThan`, `GreaterThanOrEqual`, `LessThan`, `LessThanOrEqual`.
- **Ordering**: Add ascending or descending order by clauses.
- **Pagination**: Use `Skip` and `Take` for efficient paging of results.
- **Projections**: Select specific fields, collections, or custom types (including anonymous types and DTOs).
- **Fetching (Joins)**: Eagerly fetch related entities with `Fetch`.
- **Count Queries**: Get the count of entities matching any set of conditions.
- **Integration with NHibernate**: Seamlessly integrates with NHibernate's `ISession` and `ICriteria` APIs.
- **Type-safe Property Access**: All property references are strongly-typed, reducing runtime errors.
- **Unit Tested**: Includes a suite of unit tests for all major features.

## Getting Started

### 1. Define Your Entities

```csharp
public class Author
{
    public virtual int Key { get; set; }
    public virtual string Name { get; set; }
    public virtual string Email { get; set; }
    public virtual string Country { get; set; }
    public virtual IList<Book> Books { get; set; }
}

public class Book
{
    public virtual int Key { get; set; }
    public virtual string Title { get; set; }
    public virtual string Genre { get; set; }
    public virtual Author Author { get; set; }
}
```

### 2. Build Queries with FluentConditions

```csharp
using (ISession session = ...)
{
    var conditions = new FluentConditions<Author>()
        .Equal(x => x.Country, "India")
        .IsNotNull(x => x.Email)
        .OrderByAscending(x => x.Name)
        .Skip(10)
        .Take(10);

    var authors = session.GetList(conditions);
}
```

### 3. Projections and Custom Results

```csharp
// Select only names
conditions.Select(x => x.Name);
var names = session.GetProjectedList(conditions);

// Project to a custom DTO
conditions.Select(x => new { x.Key, x.Name, x.Email });
var dtos = session.GetProjectedList<Author, SimpleResult>(conditions);
```

### 4. Fetching Related Entities

```csharp
conditions.Fetch(x => x.Books);
```

### 5. Count Queries

```csharp
int count = session.GetCount(conditions);
```

## Supported Condition Methods

- `Equal`, `NotEqual`
- `IsIn`, `IsNotIn`
- `IsNull`, `IsNotNull`
- `Between`
- `Like`, `NotLike`
- `GreaterThan`, `GreaterThanOrEqual`, `LessThan`, `LessThanOrEqual`
- `OrderByAscending`, `OrderByDescending`
- `Select` (projections)
- `Fetch` (joins)
- `Skip`, `Take` (pagination)

## Example Usage

See `NHQueryBuilder.Example/Program.cs` for comprehensive usage examples, including projections, joins, and pagination.

## Requirements
- .NET 8
- NHibernate

## License
MIT License
