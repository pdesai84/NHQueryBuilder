namespace NHQueryBuilder.DatabaseSetup.Entities
{
    public class Author
    {
        public virtual int Key { get; set; }
        public virtual string Name { get; set; } = string.Empty;
        public virtual DateTime BirthDate { get; set; }
        public virtual string City { get; set; } = string.Empty;
        public virtual string Country { get; set; } = string.Empty;
        public virtual string Phone { get; set; } = string.Empty;
        public virtual string Email { get; set; } = string.Empty;
        public virtual string Nationality { get; set; } = string.Empty;
        public virtual IList<Book> Books { get; set; } = new List<Book>();
    }
}