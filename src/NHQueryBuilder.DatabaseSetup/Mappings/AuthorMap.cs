using FluentNHibernate.Mapping;
using NHQueryBuilder.DatabaseSetup.Entities;

namespace NHQueryBuilder.DatabaseSetup.Mappings
{
    internal class AuthorMap : ClassMap<Author>
    {
        public AuthorMap()
        {
            Table("Authors");

            Id(x => x.Key).GeneratedBy.Identity();
            Map(x => x.Name).Not.Nullable();
            Map(x => x.BirthDate).Not.Nullable();
            Map(x => x.City);
            Map(x => x.Country);
            Map(x => x.Phone);
            Map(x => x.Email);
            Map(x => x.Nationality);

            HasMany(x => x.Books)
                .KeyColumn("AuthorId")
                .Inverse()
                .Cascade.All();
        }
    }
}