namespace NHQueryBuilder.DatabaseSetup.Entities
{
    public class Book
    {
        public virtual int Key { get; set; }
        public virtual string Title { get; set; } = string.Empty;
        public virtual string ISBN { get; set; } = string.Empty;
        public virtual int PublishedYear { get; set; }
        public virtual Author Author { get; set; } = null!;
        public virtual string Genre { get; set; } = string.Empty;
        public virtual bool IsAvailable { get; set; }
        public virtual IList<Loan> Loans { get; set; } = new List<Loan>();
    }
}