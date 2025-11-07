using FluentNHibernate.Mapping;
using NHQueryBuilder.DatabaseSetup.Entities;

namespace NHQueryBuilder.DatabaseSetup.Mappings
{
    internal class LoanMap : ClassMap<Loan>
    {
        public LoanMap()
        {
            Table("Loans");
            Id(x => x.Key).GeneratedBy.Identity();
            References(x => x.Book).Column("BookId").Not.Nullable();
            References(x => x.Borrower).Column("BorrowerId").Not.Nullable();
            Map(x => x.LoanDate).Not.Nullable();
            Map(x => x.ReturnDate).Nullable();
        }
    }
}