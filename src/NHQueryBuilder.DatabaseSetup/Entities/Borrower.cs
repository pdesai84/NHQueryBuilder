namespace NHQueryBuilder.DatabaseSetup.Entities
{
    public class Borrower
    {
        public virtual int Key { get; set; }
        public virtual string Name { get; set; } = string.Empty;
        public virtual string Email { get; set; } = string.Empty;
        public virtual IList<Loan> Loans { get; set; } = new List<Loan>();
    }
}