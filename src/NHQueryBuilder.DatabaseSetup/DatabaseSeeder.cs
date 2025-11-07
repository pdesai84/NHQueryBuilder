using NHibernate;
using NHQueryBuilder.DatabaseSetup.Entities;
using NHQueryBuilder.DatabaseSetup.Helper;

namespace NHQueryBuilder.DatabaseSetup
{
    public static class DatabaseSeeder
    {
        public static void SeedDynamicData(ISessionFactory sessionFactory)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    // Authors
                    var authors = new List<Author>();
                    for (int i = 1; i <= BogusHelper.RandomNumber(20, 75); i++)
                    {
                        authors.Add(new Author
                        {
                            Name = BogusHelper.RandomName,
                            City = BogusHelper.RandomCity,
                            Country = BogusHelper.RandomCountry,
                            Phone = BogusHelper.RandomPhone,
                            Email = BogusHelper.RandomEmail,
                            Nationality = BogusHelper.RandomNationality,
                            BirthDate = BogusHelper.DateBetween(new DateTime(1940, 1, 1), new DateTime(1995, 12, 31))
                        });
                    }

                    // Books
                    var genres = new[] { "Romance", "Sci-Fi", "Mystery", "Fantasy", "Non-Fiction", "Dystopian", "Thriller", "Biography" };
                    var books = new List<Book>();
                    var rand = new Random(42);
                    for (int i = 1; i <= 250; i++)
                    {
                        Author author = authors[rand.Next(authors.Count)];
                        var book = new Book
                        {
                            Title = BogusHelper.BookName,
                            PublishedYear = 1950 + rand.Next(70),
                            ISBN = BogusHelper.RandomNumberAsString(10),
                            Genre = genres[rand.Next(genres.Length)],
                            IsAvailable = rand.NextDouble() > 0.2,
                            Author = author
                        };
                        books.Add(book);
                    }

                    // Borrowers
                    var borrowers = new List<Borrower>();
                    for (int i = 1; i <= 20; i++)
                    {
                        borrowers.Add(new Borrower
                        {
                            Name = BogusHelper.RandomName,
                            Email = BogusHelper.RandomEmail
                        });
                    }

                    // Loans
                    var loans = new List<Loan>();
                    for (int i = 1; i <= 100; i++)
                    {
                        Book? book = books[rand.Next(books.Count)];
                        Borrower? borrower = borrowers[rand.Next(borrowers.Count)];
                        DateTime loanDate = DateTime.Today.AddDays(-rand.Next(60));
                        DateTime? returnDate = (i % 3 == 0) ? null : loanDate.AddDays(rand.Next(1, 30));
                        var loan = new Loan
                        {
                            Book = book,
                            Borrower = borrower,
                            LoanDate = loanDate,
                            ReturnDate = returnDate
                        };
                        loans.Add(loan);
                    }

                    // Save all authors
                    foreach (Author author in authors)
                        session.Save(author);

                    // Save all borrowers
                    foreach (Borrower borrower in borrowers)
                        session.Save(borrower);

                    // Save all books
                    foreach (Book book in books)
                        session.Save(book);

                    // Save all loans
                    foreach (Loan loan in loans)
                        session.Save(loan);

                    tx.Commit();
                }
            }
        }

        public static void SeedStaticData(ISessionFactory sessionFactory)
        {
            string[] Nationalities = new[] { "American", "British", "Canadian", "French", "German", "Italian", "Spanish", "Japanese", "Indian" };
            string[] Countries = new[] { "USA", "UK", "Canada", "France", "Germany", "Italy", "Spain", "Japan", "India" };

            using (var session = sessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    // Authors
                    var authors = new List<Author>();
                    for (int i = 1; i <= BogusHelper.RandomNumber(20, 75); i++)
                    {
                        authors.Add(new Author
                        {
                            Name = $"Author {i}",
                            City = BogusHelper.RandomCity,
                            Country = BogusHelper.PickRandom(Countries),
                            Phone = BogusHelper.RandomPhone,
                            Email = BogusHelper.RandomEmail,
                            Nationality = BogusHelper.PickRandom(Nationalities),
                            BirthDate = BogusHelper.DateBetween(new DateTime(1940, 1, 1), new DateTime(1995, 12, 31))
                        });
                    }

                    // Books
                    var genres = new[] { "Romance", "Sci-Fi", "Mystery", "Fantasy", "Non-Fiction", "Dystopian", "Thriller", "Biography" };
                    var books = new List<Book>();
                    var rand = new Random(42);
                    for (int i = 1; i <= 250; i++)
                    {
                        Author author = BogusHelper.PickRandom(authors);
                        var book = new Book
                        {
                            Title = $"Book {i}",
                            PublishedYear = (int) BogusHelper.RandomNumber(1950, 2020),
                            ISBN = BogusHelper.RandomNumberAsString(10),
                            Genre = BogusHelper.PickRandom(genres),
                            IsAvailable = rand.NextDouble() > 0.2,
                            Author = author
                        };
                        books.Add(book);
                    }

                    // Borrowers
                    var borrowers = new List<Borrower>();
                    for (int i = 1; i <= 20; i++)
                    {
                        borrowers.Add(new Borrower
                        {
                            Name = $"Borrower {i}",
                            Email = BogusHelper.RandomEmail
                        });
                    }

                    // Loans
                    var loans = new List<Loan>();
                    for (int i = 1; i <= 100; i++)
                    {
                        Book? book = BogusHelper.PickRandom(books);
                        Borrower? borrower = BogusHelper.PickRandom(borrowers);
                        DateTime loanDate = DateTime.Today.AddDays(-rand.Next(60));
                        DateTime? returnDate = (i % 3 == 0) ? null : loanDate.AddDays(rand.Next(1, 30));
                        var loan = new Loan
                        {
                            Book = book,
                            Borrower = borrower,
                            LoanDate = loanDate,
                            ReturnDate = returnDate
                        };
                        loans.Add(loan);
                    }

                    // Save all authors
                    foreach (Author author in authors)
                        session.Save(author);

                    // Save all borrowers
                    foreach (Borrower borrower in borrowers)
                        session.Save(borrower);

                    // Save all books
                    foreach (Book book in books)
                        session.Save(book);

                    // Save all loans
                    foreach (Loan loan in loans)
                        session.Save(loan);

                    tx.Commit();
                }
            }
        }
    }
}
