using NHibernate;
using NHibernate.Tool.hbm2ddl;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;

namespace NHQueryBuilder.DatabaseSetup
{
    internal class NhFactory
    {
        private static ISessionFactory? _sessionFactory;
        public static ISessionFactory GetSessionFactory(string? dbFilePath = null)
        {
            if (_sessionFactory != null)
                return _sessionFactory;

            var connectionString = $"Data Source={dbFilePath};Version=3;New=True;";

            var nHibernateConfig = new Configuration();
            nHibernateConfig.DataBaseIntegration(db =>
            {
                db.Dialect<NHibernate.Dialect.SQLiteDialect>();
                db.Driver<NHibernate.Driver.SQLite20Driver>();
                db.ConnectionString = connectionString;
            });

            nHibernateConfig.SetProperty(NHibernate.Cfg.Environment.ShowSql, true.ToString());

            FluentConfiguration configuration = Fluently.Configure(nHibernateConfig)
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<NHQueryBuilder.DatabaseSetup.Mappings.AuthorMap>())
                .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true));

            _sessionFactory = configuration.BuildSessionFactory();
            return _sessionFactory;
        }
    }
}
