namespace NHQueryBuilder.DatabaseSetup.Entities
{
    public class Loan
    {
        public virtual int Key { get; set; }
        public virtual Book Book { get; set; } = null!;
        public virtual Borrower Borrower { get; set; } = null!;
        public virtual DateTime LoanDate { get; set; }
        public virtual DateTime? ReturnDate { get; set; }
    }
}