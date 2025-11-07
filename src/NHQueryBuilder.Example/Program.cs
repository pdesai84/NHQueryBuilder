using NHibernate;
using NHibernate.Criterion;
using NHQueryBuilder;
using NHQueryBuilder.Builders;
using NHQueryBuilder.DatabaseSetup;
using NHQueryBuilder.DatabaseSetup.Entities;
using NHQueryBuilder.Example;

Session session = new Session();
session.Open(new DatabaseProfile("D:\\Temp", "1NHQueryBuilder.db"));

// Most boorowed authors in the last year
using (ISession cs = session.GetSession())
{
    FluentConditions<Loan> conditions = new FluentConditions<Loan>();
    conditions.GreaterThanOrEqual(x => x.LoanDate, DateTime.Now.AddYears(-1));
    conditions.IsIn(x => x.Book.Genre, new[] { "Sci-Fi", "Fantasy", "Dystopian" });

    var list = cs.GetList(conditions);
}

// More complex example with pagination and ordering
using (ISession cs = session.GetSession())
{
    FluentConditions<Loan> conditions = new FluentConditions<Loan>();
    conditions.Between(x => x.LoanDate, DateTime.Now.AddDays(-90), DateTime.Now);
    conditions.IsIn(x => x.Book.Genre, new[] { "Sci-Fi", "Fantasy", "Dystopian" });
    conditions.IsNotNull(x => x.Borrower.Name);
    conditions.Fetch(x => x.Book.Author);
    conditions.Skip(10).Take(10);
    conditions.OrderByAscending(x => x.Book.Title);

    var list = cs.GetList(conditions);
}

// Simple projection
using (ISession cs = session.GetSession())
{
    FluentConditions<Author> conditions = new FluentConditions<Author>();
    conditions.IsNotNull(x => x.Email);
    conditions.Equal(x => x.Country, "India");
    conditions.Select(x => x.Name);
    
    var authors = cs.GetProjectedList(conditions);
}

// Projection return as list of objects
using (ISession cs = session.GetSession())
{
    FluentConditions<Author> conditions = new FluentConditions<Author>();
    conditions.GreaterThan(x => x.Key, 10);
    conditions.Select(x => x.Books);

    // The following will return as list of objects
    var booksAsObjects = cs.GetProjectedList(conditions);
}

// Projection with type transformation
using (ISession cs = session.GetSession())
{
    FluentConditions<Author> conditions = new FluentConditions<Author>();
    conditions.GreaterThan(x => x.Key, 10);
    conditions.Select(x => x.Books);

    // The following will return as list of Books. Transforming the objects into Book type
    var books = cs.GetProjectedList<Author, Book>(conditions);
}

using (ISession cs = session.GetSession())
{
    FluentConditions<Book> conditions = new FluentConditions<Book>();
    conditions.GreaterThan(x => x.Key, 10);
    conditions.Select(x => x.Author);

    // The following will return as list of Books. Transforming the objects into Book type
    var books = cs.GetProjectedList<Book, Author>(conditions);
}

// Custom type projection
using (ISession cs = session.GetSession())
{
    FluentConditions<Author> conditions = new FluentConditions<Author>();
    conditions.GreaterThan(x => x.Key, 10);
    conditions.Select(x => new { x.Key, x.Name, x.Email });

    // The following will return as list of Books. Transforming the objects into Book type
    var books = cs.GetProjectedList<Author, SimpleResult>(conditions);
}