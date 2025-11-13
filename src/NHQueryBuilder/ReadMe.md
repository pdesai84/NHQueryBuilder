# NHQueryBuilder

**NHQueryBuilder** is a fluent, strongly-typed query builder for **NHibernate** that simplifies building complex queries in .NET.  
It provides a modern, expressive API for filtering, ordering, projections, pagination, and more — all type-safe and fully integrated with NHibernate.

---

## Key Features

* **Fluent API for Query Construction** - Chain readable, type-safe methods.
* **Comprehensive Condition Support** - `Equal`, `NotEqual`, `IsIn`, `Between`, `Like`, `IsNull`, `GreaterThan`, `LessThan`, etc.
* **Ordering & Pagination** - `OrderByAscending`, `OrderByDescending`, `Skip`, `Take`.
* **Projections (Select)** - Fetch only specific fields or map to DTOs.
* **Joins (Fetch)** - Eagerly load related entities.
* **Count Queries** - Get total record counts for conditions.
* **Integrated with NHibernate** - Works seamlessly with `ISession`.
* **Extensible Design** - Easily add new SQL functions or logic.

---

## Getting Started

### Define Entities

```csharp
public class Author
{
    public virtual int Key { get; set; }
    public virtual string Name { get; set; }
    public virtual string Country { get; set; }
    public virtual string Nationality { get; set; }
    public virtual IList<Book> Books { get; set; }
}

public class Book
{
    public virtual int Key { get; set; }
    public virtual string Title { get; set; }
    public virtual Author Author { get; set; }
}
```

### Build a Query

```csharp
var cond = new FluentConditions<Author>()
    .Equal(x => x.Country, "India")
    .IsIn(x => x.Nationality, new[] { "American", "Indian", "Canadian" })
    .IsNotNull(x => x.Name)
    .OrderByAscending(x => x.Name)
    .Skip(10)
    .Take(10);

var authors = session.GetList(cond);
```

### Select Only What You Need

```csharp
cond.Select(x => new { x.Key, x.Name });
var results = session.GetProjectedList(cond);
```

### Use transformer to map the results

```csharp
cond.Select(x => new { x.Key, x.Name, x.Email });
var books = cs.GetProjectedList<Author, SimpleResult>(cond);

// Another way
FluentConditions<Book> conditions = new FluentConditions<Book>();
conditions.GreaterThan(x => x.Key, 10);
conditions.Select(x => x.Author);
var books = session.GetProjectedList<Book, Author>(conditions);
```

### Use OR Conditions

```csharp
cond.Equal(x => x.Country, "India").OR.Equal(x => x.Country, "Belgium");
```

---

## Supported Methods

| Category        | Methods                                                         |
| --------------- | --------------------------------------------------------------- |
| **Comparison**  | `Equal`, `NotEqual`, `Between`, `GreaterThan`, `LessThan`, etc. |
| **Null Checks** | `IsNull`, `IsNotNull`                                           |
| **Membership**  | `IsIn`, `NotIn`                                                 |
| **String**      | `Like`, `NotLike`                                               |
| **Projection**  | `Select`                                                        |
| **Joins**       | `Fetch`                                                         |
| **Ordering**    | `OrderByAscending`, `OrderByDescending`                         |
| **Pagination**  | `Skip`, `Take`                                                  |
| **Logical**     | `AND`, `OR`                                                     |

---

## Unit Tests & Examples

NHQueryBuilder includes **NUnit-based tests** and **example projects** demonstrating:

* Building dynamic conditions
* Combining multiple joins
* Executing projections
* Applying pagination and sorting

See `NHQueryBuilder.Example` and `NHQueryBuilder.UnitTest` projects in the GitHub repository.

---

## Community & Support

We'd love to hear your thoughts and feedback!

* **Start a Discussion:** [Join or start a conversation](https://github.com/pdesai84/NHQueryBuilder/discussions)
* **Report an Issue:** [Report a bug or request a feature](https://github.com/pdesai84/NHQueryBuilder/issues)
* **GitHub Repository:** [Visit NHQueryBuilder on GitHub](https://github.com/pdesai84/NHQueryBuilder)

If you find this project useful, please **star the repository** to show your support!

---

## License

This project is licensed under the **MIT License**.
See the [LICENSE](https://github.com/pdesai84/NHQueryBuilder/blob/main/LICENSE.txt) file for details.

---

### Acknowledgements

Built for the NHibernate community - simplifying advanced queries, one fluent line at a time.

---

## NuGet Metadata

| Metadata            | Value                                                                                            |
| ------------------- | ------------------------------------------------------------------------------------------------ |
| **Authors**         | Prahsant Desai                                                                                      |
| **Project URL**     | [https://github.com/pdesai84/NHQueryBuilder](https://github.com/pdesai84/NHQueryBuilder)         |
| **License**         | MIT                                                                                              |
| **Tags**            | NHibernate, QueryBuilder, Fluent, ORM, SQL, LINQ, C#, .NET                                       |
| **Repository Type** | GitHub                                                                                           |
| **Repository URL**  | [https://github.com/pdesai84/NHQueryBuilder.git](https://github.com/pdesai84/NHQueryBuilder.git) |

---