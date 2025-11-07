using FluentNHibernate.Mapping;
using NHQueryBuilder.DatabaseSetup.Entities;

namespace NHQueryBuilder.DatabaseSetup.Mappings
{
    internal class BorrowerMap : ClassMap<Borrower>
    {
        public BorrowerMap()
        {
            Table("Borrowers");

            Id(x => x.Key).GeneratedBy.Identity();
            Map(x => x.Name).Not.Nullable();
            Map(x => x.Email).Not.Nullable();

            HasMany(x => x.Loans)
                .KeyColumn("BorrowerId")
                .Inverse()
                .Cascade.All();
        }
    }
}