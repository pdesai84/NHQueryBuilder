using FluentNHibernate.Mapping;
using NHQueryBuilder.DatabaseSetup.Entities;

namespace NHQueryBuilder.DatabaseSetup.Mappings
{
    internal class BookMap : ClassMap<Book>
    {
        public BookMap()
        {
            Table("Books");
            Id(x => x.Key).GeneratedBy.Identity();
            Map(x => x.Title).Not.Nullable();
            Map(x => x.ISBN).Not.Nullable();
            Map(x => x.PublishedYear).Nullable();
            Map(x => x.Genre).Not.Nullable();
            Map(x => x.IsAvailable).Not.Nullable();
            References(x => x.Author).Column("AuthorId").Not.Nullable();

            HasMany(x => x.Loans)
                .KeyColumn("BookId")
                .Inverse()
                .Cascade.All();
        }
    }
}